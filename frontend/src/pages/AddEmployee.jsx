import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { addEmployee, updateEmployee, getEmployee, getDepartments, getDesignations, getBankDetailByEmployee } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import BankDetailForm from '../components/employee/BankDetailForm';
import PayrollForm from '../components/employee/PayrollForm';
import { toast } from 'react-toastify';
import { formatDateForInput, toUtcNoonIsoString } from '../utils/dateUtils';

const AddEmployee = () => {
  const { id } = useParams();
  const isEditMode = !!id;

  const [formData, setFormData] = useState({
    employeeCode: '',
    firstName: '',
    lastName: '',
    email: '',
    contactNumber: '',
    gender: '',
    dateOfBirth: '',
    maritalStatus: 'Single',
    nationalIdNumber: '',
    departmentId: '',
    designationId: '',
    managerId: '',
    joiningDate: new Date().toISOString().split('T')[0],
    employmentType: 'Full-Time',
    isActive: true
  });

  const [departments, setDepartments] = useState([
    { id: '1', name: 'Engineering' },
    { id: '2', name: 'Marketing' },
    { id: '3', name: 'Human Resources' },
    { id: '4', name: 'Finance' },
    { id: '5', name: 'Operations' }
  ]);

  const [designations, setDesignations] = useState([
    { id: '1', title: 'Software Developer' },
    { id: '2', title: 'Project Manager' },
    { id: '3', title: 'HR Specialist' },
    { id: '4', title: 'Marketing Manager' },
    { id: '5', title: 'Financial Analyst' },
    { id: '6', title: 'Operations Manager' }
  ]);

  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();
  const { user } = useAuth();

  const [bankDetail, setBankDetail] = useState({
    bankName: '',
    accountHolderName: '',
    accountNumber: '',
    ifscCode: '',
    branchName: ''
  });

  const [payroll, setPayroll] = useState({
    basicSalary: 0,
    hra: 0,
    allowances: 0,
    deductions: 0,
    netSalary: 0,
    salaryMonth: new Date().toISOString().split('T')[0]
  });

  const [step, setStep] = useState(1); // 1: Personal Details, 2: Bank Details, 3: Salary Details

  // Fetch departments, designations, and employee data (if in edit mode)
  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);

        // Fetch departments and designations
        const [deptData, desigData] = await Promise.all([
          getDepartments(),
          getDesignations()
        ]);

        if (Array.isArray(deptData) && deptData.length > 0) {
          setDepartments(deptData);
        }

        if (Array.isArray(desigData) && desigData.length > 0) {
          setDesignations(desigData);
        }

        // If in edit mode, fetch employee data
        if (isEditMode && id) {
          const [employeeData, bankData] = await Promise.all([
            getEmployee(id),
            getBankDetailByEmployee(id)
          ]);

          if (employeeData) {
            // Split full name into first and last name
            let firstName = '';
            let lastName = '';

            if (employeeData.fullName) {
              const nameParts = employeeData.fullName.split(' ');
              firstName = nameParts[0] || '';
              lastName = nameParts.slice(1).join(' ') || '';
            }

            // Format date fields for input fields (YYYY-MM-DD)

            // Update form data with employee details
            setFormData({
              id: employeeData.id,
              employeeCode: employeeData.employeeCode || '',
              firstName,
              lastName,
              email: employeeData.email || '',
              contactNumber: employeeData.contactNumber || '',
              gender: employeeData.gender || '',
              dateOfBirth: formatDateForInput(employeeData.dateOfBirth),
              maritalStatus: employeeData.maritalStatus || 'Single',
              nationalIdNumber: employeeData.nationalIdNumber || '',
              departmentId: employeeData.departmentId || '',
              designationId: employeeData.designationId || '',
              managerId: employeeData.managerId || null,
              joiningDate: formatDateForInput(employeeData.joiningDate),
              employmentType: employeeData.employmentType || 'Full-Time',
              isActive: employeeData.isActive !== undefined ? employeeData.isActive : true
            });

            // Update bank details if available
            if (bankData && bankData.bankName) {
              setBankDetail({
                bankName: bankData.bankName || '',
                accountHolderName: bankData.accountHolderName || '',
                accountNumber: bankData.accountNumber || '',
                ifscCode: bankData.ifscCode || '',
                branchName: bankData.branchName || ''
              });
            }

            // Set step to 1 (personal details)
            setStep(1);
          }
        }

        setLoading(false);
      } catch (err) {
        console.error('Error fetching data:', err);
        setError('Failed to load data. Please try again.');
        setLoading(false);
        // We'll keep using the mock data if the API fails
      }
    };

    fetchData();
  }, [id, isEditMode]);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData(prevData => ({
      ...prevData,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      // Validate required fields
      if (!formData.firstName || !formData.lastName || !formData.email || !formData.departmentId || !formData.designationId) {
        setError('Please fill in all required fields in the Personal Details section');
        setLoading(false);
        return;
      }

      // Validate bank details
      if (!bankDetail.bankName || !bankDetail.accountHolderName || !bankDetail.accountNumber || !bankDetail.ifscCode) {
        setError('Please fill in all required fields in the Bank Details section');
        setLoading(false);
        return;
      }

      // Validate salary information
      if (payroll.basicSalary <= 0) {
        setError('Basic salary must be greater than zero');
        setLoading(false);
        return;
      }

      // Combine first and last name for fullName
      const fullName = `${formData.firstName} ${formData.lastName}`;

      // Prepare employee data with bank details and payroll
      // Ensure dates are in UTC format
      const employeeData = {
        ...formData,
        fullName,
        bankDetail,
        payroll,
        // Convert dates to UTC format
        dateOfBirth: formData.dateOfBirth ? toUtcNoonIsoString(formData.dateOfBirth) : null,
        joiningDate: formData.joiningDate ? toUtcNoonIsoString(formData.joiningDate) : toUtcNoonIsoString(new Date()),
        exitDate: formData.exitDate ? toUtcNoonIsoString(formData.exitDate) : null
      };

      console.log('Date fields converted to UTC format:', {
        dateOfBirth: employeeData.dateOfBirth,
        joiningDate: employeeData.joiningDate,
        exitDate: employeeData.exitDate
      });

      console.log('Submitting employee data:', employeeData);

      let response;
      if (isEditMode && id) {
        // Update existing employee
        console.log('Updating employee with ID:', id);
        try {
          response = await updateEmployee(id, employeeData);
          setSuccess('Employee updated successfully!');
          toast.success('Employee updated successfully!');
        } catch (updateError) {
          console.error('Error in updateEmployee:', updateError);
          if (updateError.response) {
            console.error('Error response:', updateError.response.data);
          }
          throw updateError; // Re-throw to be caught by the outer catch block
        }
      } else {
        // Add new employee
        response = await addEmployee(employeeData);
        setSuccess('Employee added successfully!');
        toast.success('Employee added successfully!');
      }

      // Reset form after successful submission
      setFormData({
        employeeCode: '',
        firstName: '',
        lastName: '',
        email: '',
        contactNumber: '',
        gender: '',
        dateOfBirth: '',
        maritalStatus: 'Single',
        nationalIdNumber: '',
        departmentId: '',
        designationId: '',
        managerId: '',
        joiningDate: formatDateForInput(new Date()),
        employmentType: 'Full-Time',
        isActive: true
      });

      setBankDetail({
        bankName: '',
        accountHolderName: '',
        accountNumber: '',
        ifscCode: '',
        branchName: ''
      });

      setPayroll({
        basicSalary: 0,
        hra: 0,
        allowances: 0,
        deductions: 0,
        netSalary: 0,
        salaryMonth: new Date().toISOString().split('T')[0]
      });

      setStep(1);

      // Redirect after a short delay
      setTimeout(() => {
        if (isEditMode) {
          // If editing, go back to employee details
          navigate(`/employees/${id}`);
        } else {
          // If adding new, go to dashboard
          navigate('/dashboard');
        }
      }, 1500);

    } catch (err) {
      console.error('Error adding employee:', err);
      setError(err.response?.data?.message || `Failed to ${isEditMode ? 'update' : 'add'} employee. Please try again.`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="max-w-4xl mx-auto">
        <div className="bg-white rounded-lg shadow-md overflow-hidden">
          <div className="bg-gradient-to-r from-blue-800 to-indigo-900 px-6 py-4">
            <h1 className="text-white text-xl font-semibold">
              {isEditMode ? 'Edit Employee' : 'Add New Employee'}
            </h1>
          </div>

          {/* Step Indicator */}
          <div className="px-6 pt-4">
            <div className="flex items-center justify-between mb-4">
              <div className="flex items-center">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center ${step >= 1 ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-600'}`}>
                  1
                </div>
                <div className={`h-1 w-12 ${step > 1 ? 'bg-indigo-600' : 'bg-gray-200'} mx-2`}></div>
                <div className={`w-8 h-8 rounded-full flex items-center justify-center ${step >= 2 ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-600'}`}>
                  2
                </div>
                <div className={`h-1 w-12 ${step > 2 ? 'bg-indigo-600' : 'bg-gray-200'} mx-2`}></div>
                <div className={`w-8 h-8 rounded-full flex items-center justify-center ${step >= 3 ? 'bg-indigo-600 text-white' : 'bg-gray-200 text-gray-600'}`}>
                  3
                </div>
              </div>
              <div className="text-sm text-gray-500">
                Step {step} of 3: {step === 1 ? 'Personal Details' : step === 2 ? 'Bank Details' : 'Salary Information'}
              </div>
            </div>
          </div>

          {error && (
            <div className="bg-red-50 border-l-4 border-red-500 p-4 mx-6 my-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <svg className="h-5 w-5 text-red-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="text-sm text-red-700">{error}</p>
                </div>
              </div>
            </div>
          )}

          {success && (
            <div className="bg-green-50 border-l-4 border-green-500 p-4 mx-6 my-4">
              <div className="flex">
                <div className="flex-shrink-0">
                  <svg className="h-5 w-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="text-sm text-green-700">{success}</p>
                </div>
              </div>
            </div>
          )}

          <form onSubmit={handleSubmit} className="p-6">
            {/* Step 1: Personal Details */}
            {step === 1 && (
              <div className="space-y-6">
                {/* Personal details form fields */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="form-group">
                    <label htmlFor="employeeCode" className="block text-sm font-medium text-gray-700 mb-1">Employee Code</label>
                    <input
                      type="text"
                      id="employeeCode"
                      name="employeeCode"
                      value={formData.employeeCode}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      placeholder="Leave blank for auto-generation"
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-1">First Name <span className="text-red-500">*</span></label>
                    <input
                      type="text"
                      id="firstName"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-1">Last Name <span className="text-red-500">*</span></label>
                    <input
                      type="text"
                      id="lastName"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">Email <span className="text-red-500">*</span></label>
                    <input
                      type="email"
                      id="email"
                      name="email"
                      value={formData.email}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="contactNumber" className="block text-sm font-medium text-gray-700 mb-1">Contact Number</label>
                    <input
                      type="tel"
                      id="contactNumber"
                      name="contactNumber"
                      value={formData.contactNumber}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="gender" className="block text-sm font-medium text-gray-700 mb-1">Gender</label>
                    <select
                      id="gender"
                      name="gender"
                      value={formData.gender}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="">Select Gender</option>
                      <option value="Male">Male</option>
                      <option value="Female">Female</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>

                  <div className="form-group">
                    <label htmlFor="dateOfBirth" className="block text-sm font-medium text-gray-700 mb-1">Date of Birth</label>
                    <input
                      type="date"
                      id="dateOfBirth"
                      name="dateOfBirth"
                      value={formData.dateOfBirth}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="maritalStatus" className="block text-sm font-medium text-gray-700 mb-1">Marital Status</label>
                    <select
                      id="maritalStatus"
                      name="maritalStatus"
                      value={formData.maritalStatus}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="Single">Single</option>
                      <option value="Married">Married</option>
                      <option value="Divorced">Divorced</option>
                      <option value="Widowed">Widowed</option>
                    </select>
                  </div>

                  <div className="form-group">
                    <label htmlFor="nationalIdNumber" className="block text-sm font-medium text-gray-700 mb-1">National ID Number</label>
                    <input
                      type="text"
                      id="nationalIdNumber"
                      name="nationalIdNumber"
                      value={formData.nationalIdNumber}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                  </div>

                  {/* Employment Information Section */}
                  <div className="md:col-span-2 mt-4">
                    <h2 className="text-lg font-semibold text-gray-700 border-b pb-2 mb-4">Employment Information</h2>
                  </div>

                  <div className="form-group">
                    <label htmlFor="departmentId" className="block text-sm font-medium text-gray-700 mb-1">Department <span className="text-red-500">*</span></label>
                    <select
                      id="departmentId"
                      name="departmentId"
                      value={formData.departmentId}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    >
                      <option value="">Select Department</option>
                      {departments.map(dept => (
                        <option key={dept.id} value={dept.id}>
                          {dept.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label htmlFor="designationId" className="block text-sm font-medium text-gray-700 mb-1">Designation <span className="text-red-500">*</span></label>
                    <select
                      id="designationId"
                      name="designationId"
                      value={formData.designationId}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    >
                      <option value="">Select Designation</option>
                      {designations.map(pos => (
                        <option key={pos.id} value={pos.id}>
                          {pos.title}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label htmlFor="joiningDate" className="block text-sm font-medium text-gray-700 mb-1">Joining Date <span className="text-red-500">*</span></label>
                    <input
                      type="date"
                      id="joiningDate"
                      name="joiningDate"
                      value={formData.joiningDate}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="employmentType" className="block text-sm font-medium text-gray-700 mb-1">Employment Type</label>
                    <select
                      id="employmentType"
                      name="employmentType"
                      value={formData.employmentType}
                      onChange={handleChange}
                      className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    >
                      <option value="Full-Time">Full-Time</option>
                      <option value="Part-Time">Part-Time</option>
                      <option value="Contract">Contract</option>
                      <option value="Intern">Intern</option>
                    </select>
                  </div>

                  <div className="form-group md:col-span-2 flex items-center mt-2">
                    <input
                      type="checkbox"
                      id="isActive"
                      name="isActive"
                      checked={formData.isActive}
                      onChange={handleChange}
                      className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                    />
                    <label htmlFor="isActive" className="ml-2 block text-sm text-gray-700">
                      Active Employee
                    </label>
                  </div>
                </div>

                <div className="flex justify-end space-x-4 mt-6">
                  <button
                    type="button"
                    onClick={() => setStep(2)}
                    className="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                  >
                    Next: Bank Details
                  </button>
                </div>
              </div>
            )}

            {/* Step 2: Bank Details */}
            {step === 2 && (
              <div>
                <BankDetailForm
                  bankDetail={bankDetail}
                  setBankDetail={setBankDetail}
                />

                <div className="flex justify-between mt-6">
                  <button
                    type="button"
                    onClick={() => setStep(1)}
                    className="border border-gray-300 bg-white text-gray-700 px-4 py-2 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                  >
                    Back
                  </button>
                  <button
                    type="button"
                    onClick={() => setStep(3)}
                    className="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                  >
                    Next: Salary Information
                  </button>
                </div>
              </div>
            )}

            {/* Step 3: Salary Information */}
            {step === 3 && (
              <div>
                <PayrollForm
                  payroll={payroll}
                  setPayroll={setPayroll}
                />

                <div className="flex justify-between mt-6">
                  <button
                    type="button"
                    onClick={() => setStep(2)}
                    className="border border-gray-300 bg-white text-gray-700 px-4 py-2 rounded-md hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                  >
                    Back
                  </button>
                  <button
                    type="submit"
                    disabled={loading}
                    className={`${loading ? 'bg-indigo-400' : 'bg-indigo-600 hover:bg-indigo-700'} text-white px-4 py-2 rounded-md focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2`}
                  >
                    {loading ? 'Saving...' : 'Save Employee'}
                  </button>
                </div>
              </div>
            )}
          </form>
        </div>
      </div>
    </div>
  );
};

export default AddEmployee;
