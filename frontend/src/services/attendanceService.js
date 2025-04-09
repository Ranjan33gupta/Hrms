import axios from 'axios';

const API_URL = 'http://localhost:5171/api';

// Create an axios instance with proper configuration
const attendanceApi = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Add token to requests if available
attendanceApi.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

// Get today's attendance for employee
export const getTodayAttendance = async (employeeId) => {
  try {
    console.log(`Fetching attendance for employee ${employeeId} from ${API_URL}/Attendance/Employee/${employeeId}/Today`);
    const response = await attendanceApi.get(`/Attendance/Employee/${employeeId}/Today`);
    return response.data;
  } catch (error) {
    if (error.response && error.response.status === 404) {
      // Not found is expected if employee hasn't clocked in yet
      return null;
    }
    console.error('Error fetching today\'s attendance:', error);
    throw error;
  }
};

// Clock In with location
export const clockIn = async (employeeId, location) => {
  try {
    console.log('Clock in request for employee:', employeeId);
    console.log('Location data:', location);

    // Ensure location data is valid, use defaults if not
    const safeLocation = location || {
      latitude: 0,
      longitude: 0,
      address: 'Location not available',
      timestamp: new Date().toISOString() // Already in UTC format
    };

    const payload = {
      employeeId,
      checkInLocation: safeLocation.address || 'Location not available',
      checkInLatitude: safeLocation.latitude || 0,
      checkInLongitude: safeLocation.longitude || 0,
      checkInDevice: navigator.userAgent || 'Unknown device',
      checkInIpAddress: await getIpAddress() || 'Unknown IP'
    };

    console.log('Clocking in with payload:', payload);

    // Add a timeout to the request
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 15000);

    try {
      const response = await attendanceApi.post('/Attendance/ClockIn', payload, {
        signal: controller.signal
      });
      clearTimeout(timeoutId);
      console.log('Clock in successful:', response.data);
      return response.data;
    } catch (error) {
      clearTimeout(timeoutId);
      if (error.name === 'AbortError') {
        console.error('Clock in request timed out');
        throw new Error('Request timed out. Please try again.');
      }
      throw error;
    }
  } catch (error) {
    console.error('Error during clock in:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
      throw new Error(error.response.data || 'Failed to clock in. Server error.');
    } else if (error.request) {
      console.error('No response received:', error.request);
      throw new Error('No response from server. Please check your connection.');
    } else {
      throw error;
    }
  }
};

// Clock Out with location
export const clockOut = async (employeeId, location) => {
  try {
    console.log('Clock out request for employee:', employeeId);
    console.log('Location data:', location);

    // Ensure location data is valid, use defaults if not
    const safeLocation = location || {
      latitude: 0,
      longitude: 0,
      address: 'Location not available',
      timestamp: new Date().toISOString()
    };

    const payload = {
      employeeId,
      checkOutLocation: safeLocation.address || 'Location not available',
      checkOutLatitude: safeLocation.latitude || 0,
      checkOutLongitude: safeLocation.longitude || 0,
      checkOutDevice: navigator.userAgent || 'Unknown device',
      checkOutIpAddress: await getIpAddress() || 'Unknown IP'
    };

    console.log('Clocking out with payload:', payload);

    // Add a timeout to the request
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 15000);

    try {
      const response = await attendanceApi.put('/Attendance/ClockOut', payload, {
        signal: controller.signal
      });
      clearTimeout(timeoutId);
      console.log('Clock out successful:', response.data);
      return response.data;
    } catch (error) {
      clearTimeout(timeoutId);
      if (error.name === 'AbortError') {
        console.error('Clock out request timed out');
        throw new Error('Request timed out. Please try again.');
      }
      throw error;
    }
  } catch (error) {
    console.error('Error during clock out:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
      throw new Error(error.response.data || 'Failed to clock out. Server error.');
    } else if (error.request) {
      console.error('No response received:', error.request);
      throw new Error('No response from server. Please check your connection.');
    } else {
      throw error;
    }
  }
};

// Get attendance history for an employee
export const getAttendanceHistory = async (employeeId) => {
  try {
    console.log(`Fetching attendance history for employee ${employeeId}`);

    // Add a timeout to the request
    const controller = new AbortController();
    const timeoutId = setTimeout(() => controller.abort(), 15000);

    try {
      const response = await attendanceApi.get(`/Attendance/Employee/${employeeId}`, {
        signal: controller.signal
      });
      clearTimeout(timeoutId);
      console.log('Attendance history fetched successfully:', response.data);
      return response.data;
    } catch (error) {
      clearTimeout(timeoutId);
      if (error.name === 'AbortError') {
        console.error('Attendance history request timed out');
        throw new Error('Request timed out. Please try again.');
      }
      throw error;
    }
  } catch (error) {
    console.error('Error fetching attendance history:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
      throw new Error(error.response.data || 'Failed to fetch attendance history. Server error.');
    } else if (error.request) {
      console.error('No response received:', error.request);
      throw new Error('No response from server. Please check your connection.');
    } else {
      throw error;
    }
  }
};

// Helper function to get IP address
const getIpAddress = async () => {
  try {
    const response = await axios.get('https://api.ipify.org?format=json');
    return response.data.ip;
  } catch (error) {
    console.error('Error getting IP address:', error);
    return 'unknown';
  }
};

// Get current shift for employee
export const getCurrentShift = async (employeeId) => {
  try {
    console.log(`Fetching current shift for employee ${employeeId} from ${API_URL}/EmployeeShiftAssignments/Employee/${employeeId}/Current`);
    const response = await attendanceApi.get(`/EmployeeShiftAssignments/Employee/${employeeId}/Current`);
    return response.data;
  } catch (error) {
    if (error.response && error.response.status === 404) {
      // Not found is expected if employee doesn't have a shift assigned
      return null;
    }
    console.error('Error fetching current shift:', error);
    throw error;
  }
};

// Get all shifts
export const getAllShifts = async () => {
  try {
    const response = await attendanceApi.get(`/Shifts`);
    return response.data;
  } catch (error) {
    console.error('Error fetching shifts:', error);
    throw error;
  }
};

// Get location data
export const getLocationData = async () => {
  return new Promise((resolve, reject) => {
    if (!navigator.geolocation) {
      console.error('Geolocation is not supported by your browser');
      // Return default values instead of rejecting
      resolve({
        latitude: 0,
        longitude: 0,
        address: 'Location not available',
        timestamp: new Date().toISOString()
      });
      return;
    }

    const locationTimeout = setTimeout(() => {
      console.error('Geolocation request timed out');
      // Return default values on timeout
      resolve({
        latitude: 0,
        longitude: 0,
        address: 'Location timed out',
        timestamp: new Date().toISOString() // Already in UTC format
      });
    }, 10000); // 10 second timeout

    navigator.geolocation.getCurrentPosition(
      async position => {
        clearTimeout(locationTimeout);
        try {
          const { latitude, longitude } = position.coords;
          console.log('Location captured:', { latitude, longitude });

          // Default address in case reverse geocoding fails
          let address = 'Location captured';

          try {
            // Try to get address from coordinates
            const response = await axios.get(
              `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latitude}&lon=${longitude}`,
              { timeout: 5000 } // 5 second timeout for geocoding
            );

            if (response.data && response.data.display_name) {
              address = response.data.display_name;
              console.log('Address found:', address);
            }
          } catch (error) {
            console.error('Error getting address from coordinates:', error);
            // Continue with default address
          }

          const locationData = {
            latitude,
            longitude,
            address,
            timestamp: new Date().toISOString() // Already in UTC format
          };

          console.log('Returning location data:', locationData);
          resolve(locationData);
        } catch (error) {
          console.error('Error processing location:', error);
          // Return default values on error
          resolve({
            latitude: 0,
            longitude: 0,
            address: 'Error getting location',
            timestamp: new Date().toISOString() // Already in UTC format
          });
        }
      },
      error => {
        clearTimeout(locationTimeout);
        console.error('Geolocation error:', error);
        // Return default values on error
        resolve({
          latitude: 0,
          longitude: 0,
          address: `Location error: ${error.message || 'Unknown error'}`,
          timestamp: new Date().toISOString() // Already in UTC format
        });
      },
      {
        enableHighAccuracy: true,
        timeout: 10000,
        maximumAge: 0
      }
    );
  });
};
