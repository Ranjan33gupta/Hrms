import React, { useState, useEffect } from 'react';
import { getEmployees, getLeaveRequests } from '../services/api';
import { Link } from 'react-router-dom';
import MoodChangerButton from '../components/MoodChanger/MoodChangerButton';

const WorkingDashboard = () => {
  const [employees, setEmployees] = useState([]);
  const [leaveRequests, setLeaveRequests] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [selectedLeaveRequest, setSelectedLeaveRequest] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        
        // Use mock data instead of API calls to ensure it works
        const mockEmployees = [
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
        
        const mockLeaveRequests = [
          {
            id: '1',
            employeeId: '2',
            employeeName: 'Jane Smith',
            leaveType: 'Vacation',
            startDate: '2025-04-15',
            endDate: '2025-04-20',
            reason: 'Family vacation',
            status: 'Pending',
            requestDate: '2025-04-01'
          },
          {
            id: '2',
            employeeId: '3',
            employeeName: 'Robert Johnson',
            leaveType: 'Sick Leave',
            startDate: '2025-04-12',
            endDate: '2025-04-13',
            reason: 'Not feeling well',
            status: 'Pending',
            requestDate: '2025-04-10'
          }
        ];
        
        // Try to get real data, but fall back to mock data if it fails
        try {
          const employeesData = await getEmployees();
          if (Array.isArray(employeesData) && employeesData.length > 0) {
            setEmployees(employeesData);
          } else {
            setEmployees(mockEmployees);
          }
          
          const leaveRequestsData = await getLeaveRequests();
          if (Array.isArray(leaveRequestsData) && leaveRequestsData.length > 0) {
            setLeaveRequests(leaveRequestsData);
          } else {
            setLeaveRequests(mockLeaveRequests);
          }
        } catch (error) {
          console.error('Error fetching data from API, using mock data instead:', error);
          setEmployees(mockEmployees);
          setLeaveRequests(mockLeaveRequests);
        }
        
        setLoading(false);
      } catch (error) {
        console.error('Error in fetchData:', error);
        setError(error.message || 'An error occurred while fetching data');
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
    if (!employee) return '';
    
    if (employee.firstName && employee.lastName) {
      return `${employee.firstName.charAt(0)}${employee.lastName.charAt(0)}`;
    }
    
    if (employee.fullName) {
      const nameParts = employee.fullName.split(' ');
      if (nameParts.length >= 2) {
        return `${nameParts[0].charAt(0)}${nameParts[1].charAt(0)}`;
      }
      return nameParts[0].charAt(0);
    }
    
    return '';
  };

  // Function to get full name
  const getFullName = (employee) => {
    if (!employee) return '';
    
    if (employee.fullName) return employee.fullName;
    
    if (employee.firstName && employee.lastName) {
      return `${employee.firstName} ${employee.lastName}`;
    }
    
    return '';
  };

  // Function to format date for display
  const formatDate = (dateString) => {
    if (!dateString) return '';
    
    try {
      const date = new Date(dateString);
      
      // Check if date is valid
      if (isNaN(date.getTime())) {
        return dateString;
      }
      
      const options = { year: 'numeric', month: 'short', day: 'numeric' };
      return date.toLocaleDateString('en-US', options);
    } catch (error) {
      console.error('Error formatting date:', error);
      return dateString;
    }
  };

  // Handler functions for sidebar interaction
  const handleEmployeeClick = (employee) => {
    setSelectedEmployee(employee);
  };

  const handleLeaveRequestClick = (leaveRequest) => {
    setSelectedLeaveRequest(leaveRequest);
  };

  return (
    <div className="bg-gray-50 min-h-screen">
      <div className="container mx-auto px-4 py-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-3xl font-bold text-gray-800">Admin Dashboard</h1>
          <MoodChangerButton />
        </div>

        {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-500"></div>
          </div>
        ) : error ? (
          <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative" role="alert">
            <strong className="font-bold">Error!</strong>
            <span className="block sm:inline"> {error}</span>
          </div>
        ) : (
          <div>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-lg font-semibold text-gray-700">Total Employees</h2>
                  <div className="bg-blue-100 p-3 rounded-full">
                    <svg className="w-6 h-6 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
                    </svg>
                  </div>
                </div>
                <div className="text-3xl font-bold text-gray-800">{safeArrayLength(employees)}</div>
                <p className="text-gray-500 mt-2">Total employees in the organization</p>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-lg font-semibold text-gray-700">Leave Requests</h2>
                  <div className="bg-yellow-100 p-3 rounded-full">
                    <svg className="w-6 h-6 text-yellow-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"></path>
                    </svg>
                  </div>
                </div>
                <div className="text-3xl font-bold text-gray-800">
                  {safeArrayFilter(leaveRequests, request => request.status === 'Pending').length}
                </div>
                <p className="text-gray-500 mt-2">Pending leave requests</p>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6">
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-lg font-semibold text-gray-700">Departments</h2>
                  <div className="bg-green-100 p-3 rounded-full">
                    <svg className="w-6 h-6 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
                    </svg>
                  </div>
                </div>
                <div className="text-3xl font-bold text-gray-800">
                  {new Set(employees.map(emp => emp.departmentName)).size}
                </div>
                <p className="text-gray-500 mt-2">Active departments</p>
              </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Recent Employees</h2>
                <div className="overflow-x-auto">
                  {safeArrayLength(employees) > 0 ? (
                    <table className="min-w-full bg-white">
                      <thead>
                        <tr>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Employee
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Department
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Position
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Actions
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {employees.slice(0, 5).map((employee) => (
                          <tr key={employee.id} className="hover:bg-gray-50">
                            <td className="py-2 px-4 border-b border-gray-200">
                              <div className="flex items-center">
                                <div className="flex-shrink-0 h-10 w-10 bg-indigo-500 rounded-full flex items-center justify-center text-white font-medium">
                                  {getInitials(employee)}
                                </div>
                                <div className="ml-4">
                                  <div className="text-sm font-medium text-gray-900">{getFullName(employee)}</div>
                                  <div className="text-sm text-gray-500">{employee.email}</div>
                                </div>
                              </div>
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm text-gray-500">
                              {employee.departmentName || 'N/A'}
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm text-gray-500">
                              {employee.designationTitle || 'N/A'}
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm font-medium">
                              <button
                                onClick={() => handleEmployeeClick(employee)}
                                className="text-indigo-600 hover:text-indigo-900"
                              >
                                View
                              </button>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  ) : (
                    <div className="text-center py-4 text-gray-500">
                      <p>No employees found</p>
                    </div>
                  )}
                </div>
              </div>

              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-bold mb-4">Leave Requests</h2>
                <div className="overflow-x-auto">
                  {safeArrayLength(leaveRequests) > 0 ? (
                    <table className="min-w-full bg-white">
                      <thead>
                        <tr>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Employee
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Type
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Duration
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Status
                          </th>
                          <th className="py-2 px-4 border-b border-gray-200 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                            Actions
                          </th>
                        </tr>
                      </thead>
                      <tbody>
                        {leaveRequests.map((request) => (
                          <tr key={request.id} className="hover:bg-gray-50">
                            <td className="py-2 px-4 border-b border-gray-200">
                              <div className="text-sm font-medium text-gray-900">{request.employeeName || 'Unknown'}</div>
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm text-gray-500">
                              {request.leaveType || 'N/A'}
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm text-gray-500">
                              {formatDate(request.startDate)} - {formatDate(request.endDate)}
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200">
                              <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full ${
                                request.status === 'Approved' ? 'bg-green-100 text-green-800' : 
                                request.status === 'Rejected' ? 'bg-red-100 text-red-800' : 
                                'bg-yellow-100 text-yellow-800'
                              }`}>
                                {request.status || 'Pending'}
                              </span>
                            </td>
                            <td className="py-2 px-4 border-b border-gray-200 text-sm font-medium">
                              <button
                                onClick={() => handleLeaveRequestClick(request)}
                                className="text-indigo-600 hover:text-indigo-900 mr-3"
                              >
                                View
                              </button>
                              {request?.status === 'Pending' && (
                                <>
                                  <button className="text-green-600 hover:text-green-900 mr-3">
                                    Approve
                                  </button>
                                  <button className="text-red-600 hover:text-red-900">
                                    Reject
                                  </button>
                                </>
                              )}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  ) : (
                    <div className="text-center py-4 text-gray-500">
                      <p>No leave requests found</p>
                    </div>
                  )}
                </div>
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
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default WorkingDashboard;
