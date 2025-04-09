import axios from 'axios';
import { toUtcIsoString, toUtcNoonIsoString } from '../utils/dateUtils';

// Create an instance of axios with default config
const apiClient = axios.create({
  baseURL: 'http://localhost:5171/api',
  timeout: 15000, // 15 seconds timeout
  headers: {
    'Content-Type': 'application/json',
  }
});

// Add a request interceptor to include the auth token in all requests
apiClient.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    console.error('API request error:', error);
    return Promise.reject(error);
  }
);

// Add a response interceptor to handle common errors
apiClient.interceptors.response.use(
  response => response,
  error => {
    if (error.response) {
      // The request was made and the server responded with a status code
      // that falls out of the range of 2xx
      console.error('Error response:', error.response.data);

      // Handle 401 Unauthorized errors (expired or invalid token)
      if (error.response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        // Redirect to login if not already there
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }
    } else if (error.request) {
      // The request was made but no response was received
      console.error('No response received:', error.request);
    } else {
      // Something happened in setting up the request that triggered an Error
      console.error('Request error:', error.message);
    }
    return Promise.reject(error);
  }
);

// Helper function to format dates for PostgreSQL in UTC format
const formatDateForPostgres = (dateString) => {
  // For date-only fields (like birthdate), use noon UTC to avoid timezone issues
  const isoString = toUtcNoonIsoString(dateString);
  if (isoString) {
    console.log(`Converted date ${dateString} to UTC: ${isoString}`);
  }
  return isoString;
};

// Authentication APIs
export const login = async (credentials) => {
  try {
    console.log('Attempting login with credentials:', credentials.username);
    const response = await apiClient.post('/auth/login', credentials);

    // Check if we have a valid response with data
    if (response && response.data) {
      const { token, ...userData } = response.data;

      // Store token in localStorage
      if (token) {
        localStorage.setItem('token', token);

        // Create a simplified user object
        const user = {
          id: userData.id || '',
          username: userData.username || credentials.username,
          email: userData.email || '',
          role: userData.role || (credentials.username.toLowerCase() === 'admin' ? 'Admin' : 'Employee'),
          employeeId: userData.employeeId || null
        };

        // Store user data
        const userToStore = { token, user };
        localStorage.setItem('user', JSON.stringify(userToStore));

        console.log('Login successful, user data stored');
        return userToStore;
      }
    }

    throw new Error('Invalid response from server');
  } catch (error) {
    console.error('Login error:', error);
    throw error;
  }
};

export const register = async (userData) => {
  try {
    const response = await apiClient.post('/auth/register', userData);
    // Store token in localStorage
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  } catch (error) {
    console.error('Registration error:', error);
    throw error;
  }
};

export const logout = () => {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
};

// Check if user is authenticated
export const isAuthenticated = () => {
  const token = localStorage.getItem('token');
  return !!token;
};

export const getCurrentUser = () => {
  try {
    const userStr = localStorage.getItem('user');
    if (userStr) {
      const userData = JSON.parse(userStr);
      // Return the user object directly if it exists, otherwise return the parsed data
      return userData;
    }
    return null;
  } catch (error) {
    console.error('Error parsing user data:', error);
    // If there's an error parsing, clear the corrupted data
    localStorage.removeItem('user');
    return null;
  }
};

// Employee APIs
export const getEmployees = async () => {
  try {
    const response = await apiClient.get('/employees');

    console.log('Raw employee data from API:', response.data);

    // Process the data to ensure it has the right format for our components
    if (Array.isArray(response.data)) {
      return response.data.map(employee => {
        // Extract first and last name from fullName if they don't exist
        if (!employee.firstName && employee.fullName) {
          const nameParts = employee.fullName.split(' ');
          employee.firstName = nameParts[0] || '';
          employee.lastName = nameParts.slice(1).join(' ') || '';
        }

        console.log('Employee after processing:', employee);

        return employee;
      });
    }

    return response.data;
  } catch (error) {
    console.error('Error fetching employees:', error);
    throw error;
  }
};

export const getEmployee = async (id) => {
  try {
    const response = await apiClient.get(`/employees/${id}`);

    // Process the data to ensure it has the right format
    const employee = response.data;

    // Extract first and last name from fullName if they don't exist
    if (!employee.firstName && employee.fullName) {
      const nameParts = employee.fullName.split(' ');
      employee.firstName = nameParts[0] || '';
      employee.lastName = nameParts.slice(1).join(' ') || '';
    }

    // Backend DTO now provides these fields directly

    return employee;
  } catch (error) {
    console.error(`Error fetching employee ${id}:`, error);
    throw error;
  }
};

// Alias for getEmployee to maintain compatibility with Profile component
export const getEmployeeById = getEmployee;

export const addEmployee = async (employeeData) => {
  try {
    // Format the data to match the CreateEmployeeDTO expected by the backend
    const fullName = employeeData.fullName || `${employeeData.firstName || ''} ${employeeData.lastName || ''}`.trim();

    // Ensure we're using a valid department ID from the database
    let departmentId = employeeData.departmentId;
    // If the department ID doesn't match one of the valid IDs, default to IT department
    const validDepartmentIds = [
      "01234567-89ab-cdef-0123-456789abcdef", // IT
      "12345678-89ab-cdef-0123-456789abcdef", // HR
      "23456789-89ab-cdef-0123-456789abcdef"  // Finance
    ];
    if (!validDepartmentIds.includes(departmentId)) {
      departmentId = "01234567-89ab-cdef-0123-456789abcdef"; // Default to IT
    }

    // Ensure we're using a valid designation ID from the database
    let designationId = employeeData.designationId;
    // If the designation ID doesn't match one of the valid IDs, default to Software Engineer
    const validDesignationIds = [
      "34567890-89ab-cdef-0123-456789abcdef", // Software Engineer
      "45678901-89ab-cdef-0123-456789abcdef", // HR Manager
      "56789012-89ab-cdef-0123-456789abcdef"  // Financial Analyst
    ];
    if (!validDesignationIds.includes(designationId)) {
      designationId = "34567890-89ab-cdef-0123-456789abcdef"; // Default to Software Engineer
    }

    const formattedData = {
      employeeCode: employeeData.employeeCode || '',
      fullName: fullName,
      email: employeeData.email || '',
      contactNumber: employeeData.contactNumber || '',
      gender: employeeData.gender || '',
      dateOfBirth: employeeData.dateOfBirth ? formatDateForPostgres(employeeData.dateOfBirth) : null,
      maritalStatus: employeeData.maritalStatus || '',
      nationalIdNumber: employeeData.nationalIdNumber || '',
      departmentId: departmentId,
      designationId: designationId,
      managerId: employeeData.managerId || null,
      joiningDate: employeeData.joiningDate ? formatDateForPostgres(employeeData.joiningDate) : formatDateForPostgres(new Date()),
      bankDetail: employeeData.bankDetail ? {
        bankName: employeeData.bankDetail.bankName,
        accountHolderName: employeeData.bankDetail.accountHolderName,
        accountNumber: employeeData.bankDetail.accountNumber,
        ifscCode: employeeData.bankDetail.ifscCode,
        branchName: employeeData.bankDetail.branchName || ''
      } : null,
      initialSalary: employeeData.payroll ? {
        employeeName: fullName,
        salaryMonth: formatDateForPostgres(new Date(new Date().getFullYear(), new Date().getMonth(), 1)),
        basicSalary: parseFloat(employeeData.payroll.basicSalary) || 0,
        HRA: parseFloat(employeeData.payroll.hra) || 0,
        allowances: parseFloat(employeeData.payroll.allowances) || 0,
        deductions: parseFloat(employeeData.payroll.deductions) || 0,
        paymentDate: formatDateForPostgres(new Date())
      } : null
    };

    console.log('API service sending employee data:', formattedData);

    // Send the data directly to the backend
    const response = await apiClient.post('/employees', formattedData);
    console.log('Response from server:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error adding employee:', error);
    if (error.response) {
      console.error('Error status:', error.response.status);
      console.error('Error data:', error.response.data);
    }
    throw error;
  }
};

export const updateEmployee = async (id, employeeData) => {
  try {
    // Ensure id is a valid string
    const employeeId = id?.toString();
    // Format the data to match the UpdateEmployeeDTO expected by the backend
    const fullName = employeeData.fullName || `${employeeData.firstName || ''} ${employeeData.lastName || ''}`.trim();


    // Prepare the data for the API - match the UpdateEmployeeDTO format exactly
    const formattedData = {
      id: employeeId, // Using the string version of the ID
      fullName: fullName || null,
      email: employeeData.email || null,
      contactNumber: employeeData.contactNumber || null,
      gender: employeeData.gender || null,
      dateOfBirth: formatDateForPostgres(employeeData.dateOfBirth),
      maritalStatus: employeeData.maritalStatus || 'Single',
      nationalIdNumber: employeeData.nationalIdNumber || null,
      departmentId: employeeData.departmentId || null,
      designationId: employeeData.designationId || null,
      managerId: employeeData.managerId || null,
      joiningDate: formatDateForPostgres(employeeData.joiningDate),
      exitDate: formatDateForPostgres(employeeData.exitDate),
      isActive: typeof employeeData.isActive === 'boolean' ? employeeData.isActive : true,
      // Include bank details if provided
      bankDetail: employeeData.bankDetail ? {
        bankName: employeeData.bankDetail.bankName || '',
        accountNumber: employeeData.bankDetail.accountNumber || '',
        IFSCCode: employeeData.bankDetail.ifscCode || '',
        accountHolderName: employeeData.bankDetail.accountHolderName || employeeData.fullName || '',
        branchName: employeeData.bankDetail.branchName || ''
      } : null,
      // Include payroll information if provided
      initialSalary: employeeData.payroll ? {
        basicSalary: parseFloat(employeeData.payroll.basicSalary) || 0,
        HRA: parseFloat(employeeData.payroll.hra) || 0,
        allowances: parseFloat(employeeData.payroll.allowances) || 0,
        deductions: parseFloat(employeeData.payroll.deductions) || 0,
        netSalary: parseFloat(employeeData.payroll.netSalary) || 0,
        salaryMonth: employeeData.payroll.salaryMonth ? new Date(employeeData.payroll.salaryMonth) : new Date()
      } : null
    };

    // Remove any undefined values to prevent validation errors
    Object.keys(formattedData).forEach(key => {
      if (formattedData[key] === undefined) {
        delete formattedData[key];
      }
    });

    console.log('API service sending updated employee data:', formattedData);

    // Send the data to the backend
    console.log('Sending PUT request to:', `/employees/${employeeId}`);

    // Send the data directly as expected by the controller
    console.log('Sending data directly to the API');
    console.log('Formatted data with proper dates:', JSON.stringify(formattedData, null, 2));

    let response;
    try {
      // Send the update request with all data (employee, bank details, and payroll) in one call
      console.log('Sending complete employee data with bank details and payroll:', JSON.stringify(formattedData, null, 2));
      response = await apiClient.put(`/employees/${employeeId}`, formattedData);
      console.log('Update successful:', response);
    } catch (error) {
      console.error('Error updating employee:', error);
      if (error.response) {
        console.error('Error response:', error.response.status, error.response.data);

        // If there's a specific issue with dates, try with a different approach
        if (error.response.data && error.response.data.errors) {
          console.log('Attempting to fix date format issues...');

          // Create a new object with properly formatted UTC dates
          const fixedData = { ...formattedData };

          // For each date field, ensure it's in UTC format with time component set to noon UTC
          // This helps avoid timezone boundary issues
          if (employeeData.dateOfBirth) {
            fixedData.dateOfBirth = toUtcNoonIsoString(employeeData.dateOfBirth);
          }

          if (employeeData.joiningDate) {
            fixedData.joiningDate = toUtcNoonIsoString(employeeData.joiningDate);
          }

          if (employeeData.exitDate) {
            fixedData.exitDate = toUtcNoonIsoString(employeeData.exitDate);
          }

          console.log('Trying with explicit UTC date format:', JSON.stringify(fixedData, null, 2));
          response = await apiClient.put(`/employees/${employeeId}`, fixedData);
          console.log('Retry successful:', response);
        } else {
          throw error;
        }
      } else {
        throw error;
      }
    }

    return response?.data || { id: employeeId, success: true };
  } catch (error) {
    console.error('Error updating employee:', error);
    if (error.response) {
      console.error('Error status:', error.response.status);
      console.error('Error data:', error.response.data);
    }
    throw error;
  }
};

// Bulk Upload APIs
export const uploadBulkEmployees = async (employeeData) => {
  try {
    const response = await apiClient.post('/employees/bulk', employeeData);
    return response.data;
  } catch (error) {
    console.error('Bulk employee upload error:', error);
    throw error;
  }
};

export const uploadBulkDepartments = async (departmentData) => {
  try {
    const response = await apiClient.post('/departments/bulk', departmentData);
    return response.data;
  } catch (error) {
    console.error('Bulk department upload error:', error);
    throw error;
  }
};

export const uploadBulkDesignations = async (designationData) => {
  try {
    const response = await apiClient.post('/designations/bulk', designationData);
    return response.data;
  } catch (error) {
    console.error('Bulk designation upload error:', error);
    throw error;
  }
};

// Leave Request APIs
export const getLeaveRequests = async () => {
  try {
    const response = await apiClient.get('/leaverequests');

    // The backend now provides properly formatted DTOs with employeeName
    // Just ensure dates are formatted consistently for the UI
    if (Array.isArray(response.data)) {
      return response.data.map(leaveRequest => {
        // Format dates for display
        if (leaveRequest.startDate && typeof leaveRequest.startDate === 'string') {
          const startDate = new Date(leaveRequest.startDate);
          if (!isNaN(startDate.getTime())) {
            leaveRequest.startDate = startDate.toISOString().split('T')[0];
          }
        }

        if (leaveRequest.endDate && typeof leaveRequest.endDate === 'string') {
          const endDate = new Date(leaveRequest.endDate);
          if (!isNaN(endDate.getTime())) {
            leaveRequest.endDate = endDate.toISOString().split('T')[0];
          }
        }

        return leaveRequest;
      });
    }

    return response.data;
  } catch (error) {
    console.error('Error fetching leave requests:', error);
    // Return empty array if API fails (for development)
    return [];
  }
};

export const getLeaveRequestsByEmployee = async (employeeId) => {
  try {
    const response = await apiClient.get(`/leaverequests/employee/${employeeId}`);

    // Process the data to ensure dates are formatted consistently
    if (Array.isArray(response.data)) {
      return response.data.map(leaveRequest => {
        // Format dates for display
        if (leaveRequest.startDate && typeof leaveRequest.startDate === 'string') {
          const startDate = new Date(leaveRequest.startDate);
          if (!isNaN(startDate.getTime())) {
            leaveRequest.startDate = startDate.toISOString().split('T')[0];
          }
        }

        if (leaveRequest.endDate && typeof leaveRequest.endDate === 'string') {
          const endDate = new Date(leaveRequest.endDate);
          if (!isNaN(endDate.getTime())) {
            leaveRequest.endDate = endDate.toISOString().split('T')[0];
          }
        }

        return leaveRequest;
      });
    }

    return response.data;
  } catch (error) {
    console.error(`Error fetching leave requests for employee ${employeeId}:`, error);
    // Return empty array if API fails (for development)
    return [];
  }
};

export const addLeaveRequest = async (leaveRequestData) => {
  try {
    // Format the data to match the CreateLeaveRequestDTO expected by the backend
    const formattedData = {
      employeeId: leaveRequestData.employeeId,
      startDate: leaveRequestData.startDate ? new Date(leaveRequestData.startDate).toISOString() : new Date().toISOString(),
      endDate: leaveRequestData.endDate ? new Date(leaveRequestData.endDate).toISOString() : new Date().toISOString(),
      leaveType: leaveRequestData.leaveType || 'Annual',
      reason: leaveRequestData.reason || ''
    };

    console.log('API service sending leave request data:', formattedData);

    const response = await apiClient.post('/leaverequests', formattedData);
    return response.data;
  } catch (error) {
    console.error('Error adding leave request:', error);
    throw error;
  }
};

// Approve a leave request
export const approveLeaveRequest = async (leaveRequestId, approvalData = {}) => {
  try {
    const formattedData = {
      approvedBy: approvalData.approvedBy || localStorage.getItem('username') || 'Admin',
      comments: approvalData.comments || 'Approved'
    };

    console.log(`Approving leave request ${leaveRequestId}:`, formattedData);

    const response = await apiClient.put(`/leaverequests/approve/${leaveRequestId}`, formattedData);
    return response.data;
  } catch (error) {
    console.error(`Error approving leave request ${leaveRequestId}:`, error);
    throw error;
  }
};

// Reject a leave request
export const rejectLeaveRequest = async (leaveRequestId, rejectionData = {}) => {
  try {
    const formattedData = {
      approvedBy: rejectionData.approvedBy || localStorage.getItem('username') || 'Admin',
      comments: rejectionData.comments || 'Rejected'
    };

    console.log(`Rejecting leave request ${leaveRequestId}:`, formattedData);

    const response = await apiClient.put(`/leaverequests/reject/${leaveRequestId}`, formattedData);
    return response.data;
  } catch (error) {
    console.error(`Error rejecting leave request ${leaveRequestId}:`, error);
    throw error;
  }
};

// Department APIs
export const getDepartments = async () => {
  try {
    const response = await apiClient.get('/departments');
    return response.data;
  } catch (error) {
    console.error('Get departments error:', error);
    throw error;
  }
};

export const createDepartment = async (departmentData) => {
  try {
    const response = await apiClient.post('/departments', departmentData);
    return response.data;
  } catch (error) {
    console.error('Create department error:', error);
    throw error;
  }
};

export const updateDepartment = async (id, departmentData) => {
  try {
    const response = await apiClient.put(`/departments/${id}`, departmentData);
    return response.data;
  } catch (error) {
    console.error('Update department error:', error);
    throw error;
  }
};

export const deleteDepartment = async (id) => {
  try {
    const response = await apiClient.delete(`/departments/${id}`);
    return response.data;
  } catch (error) {
    console.error('Delete department error:', error);
    throw error;
  }
};

// Designation APIs
export const getDesignations = async () => {
  try {
    const response = await apiClient.get('/designations');
    return response.data;
  } catch (error) {
    console.error('Get designations error:', error);
    throw error;
  }
};

export const createDesignation = async (designationData) => {
  try {
    const response = await apiClient.post('/designations', designationData);
    return response.data;
  } catch (error) {
    console.error('Create designation error:', error);
    throw error;
  }
};

export const updateDesignation = async (id, designationData) => {
  try {
    const response = await apiClient.put(`/designations/${id}`, designationData);
    return response.data;
  } catch (error) {
    console.error('Update designation error:', error);
    throw error;
  }
};

export const deleteDesignation = async (id) => {
  try {
    const response = await apiClient.delete(`/designations/${id}`);
    return response.data;
  } catch (error) {
    console.error('Delete designation error:', error);
    throw error;
  }
};

// Settings APIs - Leave Policies
export const getLeavePolicies = async () => {
  try {
    const response = await apiClient.get('/LeavePolicies');
    return response.data;
  } catch (error) {
    console.error('Error fetching leave policies:', error);
    // Return empty array if API fails (for development)
    return [];
  }
};

export const getLeavePolicy = async (id) => {
  try {
    const response = await apiClient.get(`/LeavePolicies/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching leave policy ${id}:`, error);
    throw error;
  }
};

export const createLeavePolicy = async (policyData) => {
  try {
    const response = await apiClient.post('/LeavePolicies', policyData);
    return response.data;
  } catch (error) {
    console.error('Error creating leave policy:', error);
    throw error;
  }
};

export const updateLeavePolicy = async (id, policyData) => {
  try {
    const response = await apiClient.put(`/LeavePolicies/${id}`, policyData);
    return response.data;
  } catch (error) {
    console.error(`Error updating leave policy ${id}:`, error);
    throw error;
  }
};

export const deleteLeavePolicy = async (id) => {
  try {
    const response = await apiClient.delete(`/LeavePolicies/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error deleting leave policy ${id}:`, error);
    throw error;
  }
};

// Settings APIs - Holidays
export const getHolidays = async () => {
  try {
    const response = await apiClient.get('/Holidays');
    return response.data;
  } catch (error) {
    console.error('Error fetching holidays:', error);
    // Return empty array if API fails (for development)
    return [];
  }
};

export const getHolidaysByYear = async (year) => {
  try {
    const response = await apiClient.get(`/Holidays/Year/${year}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching holidays for year ${year}:`, error);
    throw error;
  }
};

export const getHoliday = async (id) => {
  try {
    const response = await apiClient.get(`/Holidays/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching holiday ${id}:`, error);
    throw error;
  }
};

export const createHoliday = async (holidayData) => {
  try {
    const response = await apiClient.post('/Holidays', holidayData);
    return response.data;
  } catch (error) {
    console.error('Error creating holiday:', error);
    throw error;
  }
};

export const updateHoliday = async (id, holidayData) => {
  try {
    const response = await apiClient.put(`/Holidays/${id}`, holidayData);
    return response.data;
  } catch (error) {
    console.error(`Error updating holiday ${id}:`, error);
    throw error;
  }
};

export const deleteHoliday = async (id) => {
  try {
    const response = await apiClient.delete(`/Holidays/${id}`);
    return response.data;
  } catch (error) {
    console.error(`Error deleting holiday ${id}:`, error);
    throw error;
  }
};

// Attendance API Functions
export const getAttendanceByEmployee = async (employeeId) => {
  try {
    const response = await apiClient.get(`/attendance/employee/${employeeId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching attendance:', error);
    throw error;
  }
};

export const getAttendanceByEmployeeAndDate = async (employeeId, date) => {
  try {
    const response = await apiClient.get(`/attendance/employee/${employeeId}/date/${date}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching attendance by date:', error);
    throw error;
  }
};

export const clockIn = async (employeeId, notes) => {
  try {
    const response = await apiClient.post('/attendance/clockin', { employeeId, notes });
    return response.data;
  } catch (error) {
    console.error('Error clocking in:', error);
    throw error;
  }
};

export const clockOut = async (employeeId, notes) => {
  try {
    const response = await apiClient.post('/attendance/clockout', { employeeId, notes });
    return response.data;
  } catch (error) {
    console.error('Error clocking out:', error);
    throw error;
  }
};

// Bank Details API Functions
export const getBankDetailByEmployee = async (employeeId) => {
  try {
    const response = await apiClient.get(`/bankdetails/employee/${employeeId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching bank details:', error);
    throw error;
  }
};

export const createBankDetail = async (bankDetailData) => {
  try {
    console.log('Creating bank detail with data:', bankDetailData);

    // Ensure field names match the backend model
    const formattedData = {
      employeeId: bankDetailData.employeeId,
      accountHolderName: bankDetailData.accountHolderName,
      accountNumber: bankDetailData.accountNumber,
      bankName: bankDetailData.bankName,
      branchName: bankDetailData.branchName,
      IFSCCode: bankDetailData.ifscCode // Note: Backend expects IFSCCode (uppercase)
    };

    const response = await apiClient.post('/bankdetails', formattedData);
    console.log('Bank detail created successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error creating bank details:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
    }
    throw error;
  }
};

export const updateBankDetail = async (id, bankDetailData) => {
  try {
    console.log('Updating bank detail with ID:', id, 'Data:', bankDetailData);

    // Ensure field names match the backend model
    const formattedData = {
      id: id,
      employeeId: bankDetailData.employeeId,
      accountHolderName: bankDetailData.accountHolderName,
      accountNumber: bankDetailData.accountNumber,
      bankName: bankDetailData.bankName,
      branchName: bankDetailData.branchName,
      IFSCCode: bankDetailData.ifscCode // Note: Backend expects IFSCCode (uppercase)
    };

    const response = await apiClient.put(`/bankdetails/${id}`, formattedData);
    console.log('Bank detail updated successfully');
    return response.data;
  } catch (error) {
    console.error('Error updating bank details:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
    }
    throw error;
  }
};

// Payroll API Functions
export const getPayrolls = async () => {
  try {
    const response = await apiClient.get('/payrolls');
    return response.data;
  } catch (error) {
    console.error('Error fetching payrolls:', error);
    throw error;
  }
};

export const getPayrollsByEmployee = async (employeeId) => {
  try {
    const response = await apiClient.get(`/payrolls/employee/${employeeId}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching employee payrolls:', error);
    throw error;
  }
};

export const getPayroll = async (id) => {
  try {
    const response = await apiClient.get(`/payrolls/${id}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching payroll:', error);
    throw error;
  }
};

export const createPayroll = async (payrollData) => {
  try {
    console.log('Creating payroll with data:', payrollData);

    // Ensure field names match the backend model
    const formattedData = {
      employeeId: payrollData.employeeId,
      basicSalary: parseFloat(payrollData.basicSalary),
      HRA: parseFloat(payrollData.hra), // Note: Backend expects HRA (uppercase)
      allowances: parseFloat(payrollData.allowances),
      deductions: parseFloat(payrollData.deductions),
      salaryMonth: payrollData.salaryMonth,
      paymentDate: payrollData.paymentDate
    };

    const response = await apiClient.post('/payrolls', formattedData);
    console.log('Payroll created successfully:', response.data);
    return response.data;
  } catch (error) {
    console.error('Error creating payroll:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
    }
    throw error;
  }
};

export const updatePayroll = async (id, payrollData) => {
  try {
    console.log('Updating payroll with ID:', id, 'Data:', payrollData);

    // Ensure field names match the backend model
    const formattedData = {
      id: id,
      employeeId: payrollData.employeeId,
      basicSalary: parseFloat(payrollData.basicSalary),
      HRA: parseFloat(payrollData.hra), // Note: Backend expects HRA (uppercase)
      allowances: parseFloat(payrollData.allowances),
      deductions: parseFloat(payrollData.deductions),
      salaryMonth: payrollData.salaryMonth,
      paymentDate: payrollData.paymentDate
    };

    const response = await apiClient.put(`/payrolls/${id}`, formattedData);
    console.log('Payroll updated successfully');
    return response.data;
  } catch (error) {
    console.error('Error updating payroll:', error);
    if (error.response) {
      console.error('Error response:', error.response.data);
    }
    throw error;
  }
};

// Employee History API Functions
export const getEmployeeHistories = async () => {
  try {
    const response = await apiClient.get('/employeehistories');
    return response.data;
  } catch (error) {
    console.error('Error fetching employee histories:', error);
    throw error;
  }
};

export const getEmployeeHistoriesByEmployeeId = async (employeeId) => {
  try {
    const response = await apiClient.get(`/employeehistories/employee/${employeeId}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching employee histories for employee ${employeeId}:`, error);
    throw error;
  }
};

export const getFlattenedEmployeeHistoriesByEmployeeId = async (employeeId) => {
  try {
    const response = await apiClient.get(`/employeehistories/employee/${employeeId}/flattened`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching flattened employee histories for employee ${employeeId}:`, error);
    throw error;
  }
};

// Payroll History API Functions
export const getPayrollHistories = async () => {
  try {
    const response = await apiClient.get('/payrollhistories');
    return response.data;
  } catch (error) {
    console.error('Error fetching payroll histories:', error);
    throw error;
  }
};

export const getPayrollHistoriesByEmployeeId = async (employeeId) => {
  try {
    const response = await apiClient.get(`/payrollhistories/employee/${employeeId}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching payroll histories for employee ${employeeId}:`, error);
    throw error;
  }
};

export default apiClient;

// Check if token exists on app load
const token = localStorage.getItem('token');
if (token) {
  apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}
