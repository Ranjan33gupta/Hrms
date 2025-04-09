import axios from 'axios';
import axiosRetry from 'axios-retry';

export const apiUrl = 'http://localhost:5171/api';

// Create an axios instance with default config
const apiClient = axios.create({
  baseURL: apiUrl,
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 30000, // 30 seconds timeout
});

// Configure retry logic
axiosRetry(apiClient, {
  retries: 3, // Number of retry attempts
  retryDelay: (retryCount) => {
    return retryCount * 1000; // Exponential backoff: 1s, 2s, 3s
  },
  retryCondition: (error) => {
    // Retry on network errors or 5xx server errors
    return axiosRetry.isNetworkOrIdempotentRequestError(error) || 
           (error.response && error.response.status >= 500);
  }
});

// Add a request interceptor to include the token in all requests
apiClient.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    
    // Add Accept-Language header for localization
    const language = localStorage.getItem('language') || 'en';
    config.headers['Accept-Language'] = language;
    
    return config;
  },
  error => {
    console.error('Request error:', error);
    return Promise.reject(error);
  }
);

// Add a response interceptor for error handling
apiClient.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    // Handle errors based on status code
    if (error.response) {
      // The request was made and the server responded with a status code
      // that falls out of the range of 2xx
      const { status, data } = error.response;
      
      switch (status) {
        case 401:
          // Unauthorized - clear token and redirect to login
          localStorage.removeItem('token');
          window.location.href = '/login';
          break;
          
        case 403:
          // Forbidden - user doesn't have permission
          console.error('Access forbidden:', data);
          break;
          
        case 404:
          // Not found
          console.error('Resource not found:', data);
          break;
          
        case 422:
          // Validation errors
          console.error('Validation errors:', data);
          break;
          
        case 500:
          // Server error
          console.error('Server error:', data);
          break;
          
        default:
          console.error(`Error ${status}:`, data);
      }
    } else if (error.request) {
      // The request was made but no response was received
      console.error('No response received:', error.request);
    } else {
      // Something happened in setting up the request
      console.error('Request setup error:', error.message);
    }
    
    // Add request ID to error for tracking
    if (error.response && error.response.headers && error.response.headers['x-request-id']) {
      error.requestId = error.response.headers['x-request-id'];
    }
    
    return Promise.reject(error);
  }
);

// Helper methods for common API operations
export const apiService = {
  // GET request with pagination support
  async get(url, params = {}, config = {}) {
    return apiClient.get(url, { ...config, params });
  },
  
  // POST request
  async post(url, data = {}, config = {}) {
    return apiClient.post(url, data, config);
  },
  
  // PUT request
  async put(url, data = {}, config = {}) {
    return apiClient.put(url, data, config);
  },
  
  // DELETE request
  async delete(url, config = {}) {
    return apiClient.delete(url, config);
  },
  
  // Upload files
  async upload(url, formData, onProgress = null, config = {}) {
    const uploadConfig = {
      ...config,
      headers: {
        ...config.headers,
        'Content-Type': 'multipart/form-data'
      }
    };
    
    if (onProgress) {
      uploadConfig.onUploadProgress = progressEvent => {
        const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
        onProgress(percentCompleted);
      };
    }
    
    return apiClient.post(url, formData, uploadConfig);
  }
};

export default apiClient;
