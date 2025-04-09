import { useState, useEffect } from 'react';
import { getEmployees, getLeaveRequests, approveLeaveRequest, rejectLeaveRequest } from '../services/api';
import { Link } from 'react-router-dom';
import { toast } from 'react-toastify';
import { useSidebar } from '../contexts/SidebarContext';
import EmployeeTable from '../components/employee/EmployeeTable';
import MoodChangerButton from '../components/MoodChanger/MoodChangerButton';

const Dashboard = () => {
  const [employees, setEmployees] = useState([]);
  const [leaveRequests, setLeaveRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [selectedLeaveRequest, setSelectedLeaveRequest] = useState(null);
  const { activeContent } = useSidebar();

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const employeesData = await getEmployees();
        const leaveRequestsData = await getLeaveRequests();

        // For development/testing - create mock data if the API returns empty
        let processedEmployees = Array.isArray(employeesData) ? employeesData : [];

        // Add department and designation data if missing
        processedEmployees = processedEmployees.map(employee => {
          // Ensure we have departmentName and designationTitle
          if (!employee.departmentName && employee.department) {
            employee.departmentName = typeof employee.department === 'string'
              ? employee.department
              : employee.department.name || 'Unknown Department';
          }

          if (!employee.designationTitle && employee.designation) {
            employee.designationTitle = typeof employee.designation === 'string'
              ? employee.designation
              : employee.designation.title || 'Unknown Position';
          }

          // For demo purposes, if still no department or designation, add mock data
          if (!employee.departmentName) {
            const deptOptions = ['Engineering', 'Marketing', 'HR', 'Finance', 'Operations'];
            employee.departmentName = deptOptions[Math.floor(Math.random() * deptOptions.length)];
          }

          if (!employee.designationTitle) {
            const posOptions = ['Manager', 'Developer', 'Analyst', 'Specialist', 'Director'];
            employee.designationTitle = posOptions[Math.floor(Math.random() * posOptions.length)];
          }

          return employee;
        });

        if (processedEmployees.length === 0) {
          // Add some mock employees for testing
          processedEmployees = [
            {
              id: '1',
              firstName: 'John',
              lastName: 'Doe',
              fullName: 'John Doe',
              email: 'john.doe@worknest.com',
              departmentName: 'Engineering',
              designationTitle: 'Software Developer',
              contactNumber: '555-1234',
              joiningDate: '2024-01-15'
            },
            {
              id: '2',
              firstName: 'Jane',
              lastName: 'Smith',
              fullName: 'Jane Smith',
              email: 'jane.smith@worknest.com',
              departmentName: 'Marketing',
              designationTitle: 'Marketing Manager',
              contactNumber: '555-5678',
              joiningDate: '2024-02-10'
            },
            {
              id: '3',
              firstName: 'Robert',
              lastName: 'Johnson',
              fullName: 'Robert Johnson',
              email: 'robert.johnson@worknest.com',
              departmentName: 'HR',
              designationTitle: 'HR Specialist',
              contactNumber: '555-9012',
              joiningDate: '2024-03-05'
            }
          ];
        } else {
          // Process the employee data to ensure we have the right properties
          processedEmployees = processedEmployees.map(emp => {
            // Extract first and last name from fullName if they don't exist
            if (!emp.firstName && emp.fullName) {
              const nameParts = emp.fullName.split(' ');
              emp.firstName = nameParts[0] || '';
              emp.lastName = nameParts.slice(1).join(' ') || '';
            }
            return emp;
          });
        }

        let processedLeaveRequests = Array.isArray(leaveRequestsData) ? leaveRequestsData : [];
        if (processedLeaveRequests.length === 0) {
          // Add some mock leave requests for testing
          processedLeaveRequests = [
            {
              id: '1',
              employeeId: '1',
              employeeName: 'John Doe',
              leaveType: 'Sick Leave',
              startDate: '2025-04-10',
              endDate: '2025-04-12',
              status: 'Pending',
              reason: 'Not feeling well'
            },
            {
              id: '2',
              employeeId: '2',
              employeeName: 'Jane Smith',
              leaveType: 'Vacation',
              startDate: '2025-04-15',
              endDate: '2025-04-20',
              status: 'Approved',
              reason: 'Family vacation'
            }
          ];
        } else {
          // Process leave requests to ensure they have employee names
          processedLeaveRequests = processedLeaveRequests.map(request => {
            // If employeeName is missing but we have employeeId, try to find the employee
            if (!request.employeeName && request.employeeId) {
              const employee = processedEmployees.find(emp => emp.id === request.employeeId);
              if (employee) {
                request.employeeName = getFullName(employee);
              }
            }
            return request;
          });
        }

        setEmployees(processedEmployees);
        setLeaveRequests(processedLeaveRequests);
        setLoading(false);
      } catch (err) {
        console.error('Error fetching data:', err);
        setError('Failed to load data. Please try again later.');
        setEmployees([]);
        setLeaveRequests([]);
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  // Safe access function to prevent errors
  const safeArrayLength = (arr) => {
    return Array.isArray(arr) ? arr.length : 0;
  };

  // Safe filter function to prevent errors
  const safeArrayFilter = (arr, predicate) => {
    return Array.isArray(arr) ? arr.filter(predicate) : [];
  };

  // Function to get initials from name
  const getInitials = (employee) => {
    if (employee.firstName && employee.lastName) {
      return `${employee.firstName.charAt(0)}${employee.lastName.charAt(0)}`;
    } else if (employee.fullName) {
      const parts = employee.fullName.split(' ');
      if (parts.length >= 2) {
        return `${parts[0].charAt(0)}${parts[parts.length - 1].charAt(0)}`;
      } else if (parts.length === 1) {
        return parts[0].charAt(0);
      }
    }
    return '??';
  };

  // Function to get full name
  const getFullName = (employee) => {
    if (employee.firstName && employee.lastName) {
      return `${employee.firstName} ${employee.lastName}`;
    } else if (employee.fullName) {
      return employee.fullName;
    }
    return 'Unknown';
  };

  // Function to format date for display
  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';

    try {
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return dateString;

      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
      });
    } catch (error) {
      return dateString;
    }
  };

  // Handler functions for sidebar interaction
  const handleEmployeeClick = (employee) => {
    setSelectedEmployee(employee);
    setSelectedLeaveRequest(null);
  };

  const handleLeaveRequestClick = (leaveRequest) => {
    setSelectedLeaveRequest(leaveRequest);
    setSelectedEmployee(null);
  };

  // Handler for approving a leave request
  const handleApproveLeaveRequest = async (leaveRequest) => {
    try {
      setLoading(true);
      await approveLeaveRequest(leaveRequest.id);

      // Update the local state to reflect the change
      const updatedLeaveRequests = leaveRequests.map(req => {
        if (req.id === leaveRequest.id) {
          return { ...req, status: 'Approved' };
        }
        return req;
      });

      setLeaveRequests(updatedLeaveRequests);

      // If this was the selected leave request, update it too
      if (selectedLeaveRequest && selectedLeaveRequest.id === leaveRequest.id) {
        setSelectedLeaveRequest({ ...selectedLeaveRequest, status: 'Approved' });
      }

      toast.success(`Leave request for ${leaveRequest.employeeName || 'employee'} has been approved`);
    } catch (error) {
      console.error('Error approving leave request:', error);
      toast.error('Failed to approve leave request. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  // Handler for rejecting a leave request
  const handleRejectLeaveRequest = async (leaveRequest) => {
    try {
      setLoading(true);
      await rejectLeaveRequest(leaveRequest.id);

      // Update the local state to reflect the change
      const updatedLeaveRequests = leaveRequests.map(req => {
        if (req.id === leaveRequest.id) {
          return { ...req, status: 'Rejected' };
        }
        return req;
      });

      setLeaveRequests(updatedLeaveRequests);

      // If this was the selected leave request, update it too
      if (selectedLeaveRequest && selectedLeaveRequest.id === leaveRequest.id) {
        setSelectedLeaveRequest({ ...selectedLeaveRequest, status: 'Rejected' });
      }

      toast.success(`Leave request for ${leaveRequest.employeeName || 'employee'} has been rejected`);
    } catch (error) {
      console.error('Error rejecting leave request:', error);
      toast.error('Failed to reject leave request. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">
          {activeContent === 'dashboard' && 'Dashboard Overview'}
          {activeContent === 'employees' && 'Employee Management'}
          {activeContent === 'leaveRequests' && 'Leave Request Management'}
        </h1>
        {/* Removed the Add Employee buttons */}
        {activeContent === 'leaveRequests' && (
          <Link
            to="/request-leave"
            className="bg-gradient-to-r from-blue-600 to-indigo-700 hover:from-blue-700 hover:to-indigo-800 text-white font-medium py-2 px-4 rounded-md flex items-center shadow-md transition-all duration-200"
          >
            <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
            </svg>
            Request Leave
          </Link>
        )}
      </div>

      {error && (
        <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-6 rounded shadow">
          <p>{error}</p>
        </div>
      )}

      {/* Only show stats cards on the main dashboard */}
      {activeContent === 'dashboard' && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow-md p-6 border-t-4 border-blue-500">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-blue-100 text-blue-500 mr-4">
                <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z"></path>
                </svg>
              </div>
              <div>
                <p className="text-sm text-gray-600">Total Employees</p>
                <p className="text-xl font-semibold">{loading ? '...' : safeArrayLength(employees)}</p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6 border-t-4 border-green-500">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-green-100 text-green-500 mr-4">
                <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd"></path>
                </svg>
              </div>
              <div>
                <p className="text-sm text-gray-600">Active Leave Requests</p>
                <p className="text-xl font-semibold">
                  {loading ? '...' : safeArrayFilter(leaveRequests, req => req && req.status === 'Pending').length}
                </p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6 border-t-4 border-yellow-500">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-yellow-100 text-yellow-500 mr-4">
                <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z"></path>
                </svg>
              </div>
              <div>
                <p className="text-sm text-gray-600">Approved Leaves</p>
                <p className="text-xl font-semibold">
                  {loading ? '...' : safeArrayFilter(leaveRequests, req => req && req.status === 'Approved').length}
                </p>
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6 border-t-4 border-red-500">
            <div className="flex items-center">
              <div className="p-3 rounded-full bg-red-100 text-red-500 mr-4">
                <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                  <path fillRule="evenodd" d="M13.477 14.89A6 6 0 015.11 6.524l8.367 8.368zm1.414-1.414L6.524 5.11a6 6 0 018.367 8.367zM18 10a8 8 0 11-16 0 8 8 0 0116 0z" clipRule="evenodd"></path>
                </svg>
              </div>
              <div>
                <p className="text-gray-600 text-sm">Rejected Leaves</p>
                <p className="text-xl font-semibold">
                  {loading ? '...' : safeArrayFilter(leaveRequests, req => req && req.status === 'Rejected').length}
                </p>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Main Dashboard Content - Only shown when activeContent is 'dashboard' */}
      {activeContent === 'dashboard' && (
        <div className="w-full">
          {/* Wellness Section with MoodChanger */}
          <div className="bg-white rounded-lg shadow-md p-6 mb-8 border-l-4 border-purple-500">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center">
              <div>
                <h2 className="text-xl font-bold mb-2">Employee Wellness Center</h2>
                <p className="text-gray-600 mb-4">
                  Track your mood and mental wellbeing. Share how you're feeling today.
                </p>
              </div>
              <div className="mt-4 md:mt-0">
                <MoodChangerButton />
              </div>
            </div>
          </div>

          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-bold mb-4">Welcome to WorkNest Dashboard</h2>
            <p className="text-gray-600 mb-4">
              Use the Info Log dropdown in the navigation bar to access specific sections:
            </p>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">
              <div className="border border-gray-200 rounded-lg p-4 hover:bg-blue-50 transition-colors duration-200">
                <div className="flex items-center mb-3">
                  <div className="p-3 rounded-full bg-blue-100 text-blue-500 mr-3">
                    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                      <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z"></path>
                    </svg>
                  </div>
                  <h3 className="font-semibold">Employee List</h3>
                </div>
                <p className="text-sm text-gray-600">
                  View and manage all employees in your organization. Add new employees, edit details, and more.
                </p>
              </div>
              <div className="border border-gray-200 rounded-lg p-4 hover:bg-blue-50 transition-colors duration-200">
                <div className="flex items-center mb-3">
                  <div className="p-2 rounded-full bg-green-100 text-green-500 mr-3">
                    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                      <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd"></path>
                    </svg>
                  </div>
                  <h3 className="font-semibold">Leave Requests</h3>
                </div>
                <p className="text-sm text-gray-600">
                  Manage leave requests from employees. Approve or reject requests, view history, and track leave balances.
                </p>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Sidebar and Main Content Layout */}
      <div className="flex flex-col md:flex-row gap-6">
        {/* Employee List Content */}
        {activeContent === 'employees' && (
          <div className="w-full">
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="bg-gradient-to-r from-blue-800 to-indigo-900 px-6 py-4 text-white flex justify-between items-center">
                <h2 className="text-lg font-semibold">Employee List</h2>
                <Link
                  to="/add-employee"
                  className="bg-white text-indigo-800 hover:bg-indigo-100 text-sm font-medium py-1 px-3 rounded flex items-center transition-colors duration-200"
                >
                  <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                  </svg>
                  Add New
                </Link>
              </div>
              <div className="p-6">
                {loading ? (
                  <p className="text-center py-4">Loading employees...</p>
                ) : safeArrayLength(employees) > 0 ? (
                  <EmployeeTable />
                ) : (
                  <div className="text-center py-4 text-gray-500">
                    <p className="mb-4">No employees found</p>
                    <Link
                      to="/add-employee"
                      className="inline-flex items-center bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium py-2 px-4 rounded transition-colors duration-200"
                    >
                      <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                      </svg>
                      Add First Employee
                    </Link>
                  </div>
                )}
              </div>
            </div>

            {selectedEmployee && (
              <div className="bg-white rounded-lg shadow-md p-6 mt-6">
                <h2 className="text-xl font-bold mb-4">{getFullName(selectedEmployee)}</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <p className="text-gray-500">Email</p>
                    <p>{selectedEmployee.email || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Department</p>
                    <p>{selectedEmployee.departmentName || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Position</p>
                    <p>{selectedEmployee.designationTitle || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Joining Date</p>
                    <p>{formatDate(selectedEmployee.joiningDate)}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Contact</p>
                    <p>{selectedEmployee.contactNumber || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Status</p>
                    <p>
                      <span className={`inline-block px-2 py-1 rounded-full text-xs ${selectedEmployee.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                        {selectedEmployee.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </p>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}

        {/* Leave Requests Content */}
        {activeContent === 'leaveRequests' && (
          <div className="w-full">
            <div className="bg-white rounded-lg shadow-md overflow-hidden">
              <div className="bg-gradient-to-r from-blue-800 to-indigo-900 px-6 py-4 text-white flex justify-between items-center">
                <h2 className="text-lg font-semibold">Recent Leave Requests</h2>
                <Link
                  to="/request-leave"
                  className="bg-white text-indigo-800 hover:bg-indigo-100 text-sm font-medium py-1 px-3 rounded flex items-center transition-colors duration-200"
                >
                  <svg className="w-4 h-4 mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"></path>
                  </svg>
                  New Request
                </Link>
              </div>
              <div className="p-6">
                {loading ? (
                  <p className="text-center py-4">Loading leave requests...</p>
                ) : safeArrayLength(leaveRequests) > 0 ? (
                  <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                      <thead className="bg-gray-50">
                        <tr>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Type</th>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">From</th>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">To</th>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                          <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
                        </tr>
                      </thead>
                      <tbody className="bg-white divide-y divide-gray-200">
                        {leaveRequests.map(request => (
                          <tr key={request?.id || Math.random()} className="hover:bg-gray-50">
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                              {request?.employeeName || 'Unknown Employee'}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                              {request?.leaveType || 'Leave'}</td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                              {formatDate(request?.startDate)}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                              {formatDate(request?.endDate)}
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap">
                              <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                                request?.status === 'Approved' ? 'bg-green-100 text-green-800' :
                                request?.status === 'Rejected' ? 'bg-red-100 text-red-800' :
                                'bg-yellow-100 text-yellow-800'
                              }`}>
                                {request?.status || 'Pending'}
                              </span>
                            </td>
                            <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                              <button
                                onClick={() => handleLeaveRequestClick(request)}
                                className="text-indigo-600 hover:text-indigo-900 mr-3"
                              >
                                View
                              </button>
                              {request?.status === 'Pending' && (
                                <>
                                  <button
                                    onClick={() => handleApproveLeaveRequest(request)}
                                    className="text-green-600 hover:text-green-900 mr-3"
                                    disabled={loading}
                                  >
                                    Approve
                                  </button>
                                  <button
                                    onClick={() => handleRejectLeaveRequest(request)}
                                    className="text-red-600 hover:text-red-900"
                                    disabled={loading}
                                  >
                                    Reject
                                  </button>
                                </>
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                ) : (
                  <div className="text-center py-4 text-gray-500">
                    <p>No leave requests found</p>
                  </div>
                )}
              </div>
            </div>

            {selectedLeaveRequest && (
              <div className="bg-white rounded-lg shadow-md p-6 mt-6">
                <h2 className="text-xl font-bold mb-4">Leave Request Details</h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <p className="text-gray-500">Employee</p>
                    <p>{selectedLeaveRequest.employeeName || 'Unknown Employee'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Leave Type</p>
                    <p>{selectedLeaveRequest.leaveType || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">From</p>
                    <p>{formatDate(selectedLeaveRequest.startDate)}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">To</p>
                    <p>{formatDate(selectedLeaveRequest.endDate)}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Status</p>
                    <p className={`inline-block px-2 py-1 rounded-full text-xs ${
                      selectedLeaveRequest.status === 'Approved' ? 'bg-green-100 text-green-800' :
                      selectedLeaveRequest.status === 'Rejected' ? 'bg-red-100 text-red-800' :
                      'bg-yellow-100 text-yellow-800'
                    }`}>
                      {selectedLeaveRequest.status || 'Pending'}
                    </p>
                  </div>
                  <div>
                    <p className="text-gray-500">Reason</p>
                    <p>{selectedLeaveRequest.reason || 'N/A'}</p>
                  </div>
                  <div>
                    <p className="text-gray-500">Request Date</p>
                    <p>{formatDate(selectedLeaveRequest.requestDate)}</p>
                  </div>
                  {selectedLeaveRequest.status === 'Approved' && (
                    <>
                      <div>
                        <p className="text-gray-500">Approved By</p>
                        <p>{selectedLeaveRequest.approvedBy || 'N/A'}</p>
                      </div>
                      <div>
                        <p className="text-gray-500">Approval Date</p>
                        <p>{formatDate(selectedLeaveRequest.approvalDate)}</p>
                      </div>
                    </>
                  )}
                  {selectedLeaveRequest.comments && (
                    <div className="col-span-2">
                      <p className="text-gray-500">Comments</p>
                      <p>{selectedLeaveRequest.comments}</p>
                    </div>
                  )}
                </div>

                {/* Action buttons for pending leave requests */}
                {selectedLeaveRequest.status === 'Pending' && (
                  <div className="mt-6 flex justify-end space-x-4">
                    <button
                      onClick={() => handleApproveLeaveRequest(selectedLeaveRequest)}
                      className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition-colors duration-200 flex items-center"
                      disabled={loading}
                    >
                      {loading ? (
                        <>
                          <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                          </svg>
                          Processing...
                        </>
                      ) : (
                        'Approve Request'
                      )}
                    </button>
                    <button
                      onClick={() => handleRejectLeaveRequest(selectedLeaveRequest)}
                      className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition-colors duration-200 flex items-center"
                      disabled={loading}
                    >
                      {loading ? (
                        <>
                          <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                          </svg>
                          Processing...
                        </>
                      ) : (
                        'Reject Request'
                      )}
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default Dashboard;
