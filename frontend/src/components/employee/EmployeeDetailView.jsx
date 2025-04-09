import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { getEmployee, getBankDetailByEmployee } from '../../services/api';
import { FaUser, FaEnvelope, FaPhone, FaIdCard, FaBirthdayCake, FaBuilding, FaBriefcase, FaCalendarAlt, FaUserTie, FaMoneyBillWave, FaUniversity } from 'react-icons/fa';
import { toast } from 'react-toastify';

const EmployeeDetailView = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [employee, setEmployee] = useState(null);
  const [bankDetails, setBankDetails] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        
        // Fetch employee details
        const employeeData = await getEmployee(id);
        setEmployee(employeeData);
        
        // Fetch bank details
        const bankData = await getBankDetailByEmployee(id);
        setBankDetails(bankData);
        
        setIsLoading(false);
      } catch (err) {
        console.error('Error fetching employee details:', err);
        setError('Failed to load employee details. Please try again later.');
        setIsLoading(false);
      }
    };

    if (id) {
      fetchData();
    }
  }, [id]);

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  };

  const handleEdit = () => {
    navigate(`/employees/edit/${id}`);
  };

  const handleViewHistory = () => {
    navigate(`/employees/${id}/history`);
  };

  const handleViewPayroll = () => {
    navigate(`/employees/${id}/payroll-history`);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <p className="text-gray-600">Loading employee details...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-4 rounded shadow">
        <p>{error}</p>
      </div>
    );
  }

  if (!employee) {
    return (
      <div className="bg-yellow-100 border-l-4 border-yellow-500 text-yellow-700 p-4 mb-4 rounded shadow">
        <p>Employee not found.</p>
        <Link to="/dashboard" className="text-blue-600 hover:underline mt-2 inline-block">
          Return to Dashboard
        </Link>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        {/* Header */}
        <div className="bg-gradient-to-r from-blue-800 to-indigo-900 px-6 py-4 flex justify-between items-center">
          <h1 className="text-white text-xl font-semibold">Employee Details</h1>
          <div className="space-x-2">
            <button
              onClick={handleEdit}
              className="bg-white text-indigo-800 hover:bg-indigo-100 text-sm font-medium py-1 px-3 rounded flex items-center transition-colors duration-200"
            >
              Edit Employee
            </button>
          </div>
        </div>

        {/* Employee Profile */}
        <div className="p-6">
          <div className="flex flex-col md:flex-row">
            {/* Profile Image and Basic Info */}
            <div className="md:w-1/3 mb-6 md:mb-0 flex flex-col items-center">
              <div className="w-32 h-32 rounded-full bg-gradient-to-br from-indigo-400 to-purple-500 flex items-center justify-center text-white text-4xl font-bold mb-4">
                {employee.fullName ? employee.fullName.charAt(0).toUpperCase() : 'E'}
              </div>
              <h2 className="text-2xl font-bold text-gray-800">{employee.fullName}</h2>
              <p className="text-indigo-600">{employee.designationTitle || 'N/A'}</p>
              <p className="text-gray-500">{employee.departmentName || 'N/A'}</p>
              
              <div className="mt-6 space-y-2 w-full">
                <div className="flex items-center">
                  <FaIdCard className="text-indigo-500 mr-2" />
                  <span className="text-gray-600">Employee ID:</span>
                  <span className="ml-2 font-medium">{employee.employeeCode || 'N/A'}</span>
                </div>
                <div className="flex items-center">
                  <FaCalendarAlt className="text-indigo-500 mr-2" />
                  <span className="text-gray-600">Joined:</span>
                  <span className="ml-2 font-medium">{formatDate(employee.joiningDate)}</span>
                </div>
                <div className="flex items-center">
                  <FaUserTie className="text-indigo-500 mr-2" />
                  <span className="text-gray-600">Reports To:</span>
                  <span className="ml-2 font-medium">{employee.managerName || 'N/A'}</span>
                </div>
              </div>
              
              <div className="mt-6 space-x-2">
                <button
                  onClick={handleViewHistory}
                  className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm"
                >
                  View History
                </button>
                <button
                  onClick={handleViewPayroll}
                  className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm"
                >
                  View Payroll
                </button>
              </div>
            </div>

            {/* Detailed Information */}
            <div className="md:w-2/3 md:pl-8 border-l border-gray-200">
              <h3 className="text-lg font-semibold mb-4 text-gray-800">Personal Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="flex items-start">
                  <FaEnvelope className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Email</p>
                    <p className="font-medium">{employee.email || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaPhone className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Phone</p>
                    <p className="font-medium">{employee.contactNumber || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaBirthdayCake className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Date of Birth</p>
                    <p className="font-medium">{formatDate(employee.dateOfBirth)}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaUser className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Gender</p>
                    <p className="font-medium">{employee.gender || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaIdCard className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">National ID</p>
                    <p className="font-medium">{employee.nationalIdNumber || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaUser className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Marital Status</p>
                    <p className="font-medium">{employee.maritalStatus || 'N/A'}</p>
                  </div>
                </div>
              </div>

              <h3 className="text-lg font-semibold mb-4 text-gray-800 border-t border-gray-200 pt-4">Employment Information</h3>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="flex items-start">
                  <FaBuilding className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Department</p>
                    <p className="font-medium">{employee.departmentName || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaBriefcase className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Designation</p>
                    <p className="font-medium">{employee.designationTitle || 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaCalendarAlt className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Joining Date</p>
                    <p className="font-medium">{formatDate(employee.joiningDate)}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaCalendarAlt className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Exit Date</p>
                    <p className="font-medium">{employee.exitDate ? formatDate(employee.exitDate) : 'N/A'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaUserTie className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Employment Type</p>
                    <p className="font-medium">{employee.employmentType || 'Full-Time'}</p>
                  </div>
                </div>
                <div className="flex items-start">
                  <FaUserTie className="text-indigo-500 mt-1 mr-2" />
                  <div>
                    <p className="text-sm text-gray-500">Status</p>
                    <p className="font-medium">
                      <span className={`px-2 py-1 rounded-full text-xs ${employee.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                        {employee.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </p>
                  </div>
                </div>
              </div>

              {bankDetails && (
                <>
                  <h3 className="text-lg font-semibold mb-4 text-gray-800 border-t border-gray-200 pt-4">Bank Details</h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="flex items-start">
                      <FaUniversity className="text-indigo-500 mt-1 mr-2" />
                      <div>
                        <p className="text-sm text-gray-500">Bank Name</p>
                        <p className="font-medium">{bankDetails.bankName || 'N/A'}</p>
                      </div>
                    </div>
                    <div className="flex items-start">
                      <FaUser className="text-indigo-500 mt-1 mr-2" />
                      <div>
                        <p className="text-sm text-gray-500">Account Holder</p>
                        <p className="font-medium">{bankDetails.accountHolderName || 'N/A'}</p>
                      </div>
                    </div>
                    <div className="flex items-start">
                      <FaMoneyBillWave className="text-indigo-500 mt-1 mr-2" />
                      <div>
                        <p className="text-sm text-gray-500">Account Number</p>
                        <p className="font-medium">{bankDetails.accountNumber ? '••••••' + bankDetails.accountNumber.slice(-4) : 'N/A'}</p>
                      </div>
                    </div>
                    <div className="flex items-start">
                      <FaUniversity className="text-indigo-500 mt-1 mr-2" />
                      <div>
                        <p className="text-sm text-gray-500">IFSC Code</p>
                        <p className="font-medium">{bankDetails.ifscCode || 'N/A'}</p>
                      </div>
                    </div>
                    <div className="flex items-start">
                      <FaBuilding className="text-indigo-500 mt-1 mr-2" />
                      <div>
                        <p className="text-sm text-gray-500">Branch</p>
                        <p className="font-medium">{bankDetails.branchName || 'N/A'}</p>
                      </div>
                    </div>
                  </div>
                </>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EmployeeDetailView;
