import React, { useState, useEffect } from 'react';
import { getEmployees, getBankDetailByEmployee, createBankDetail, updateBankDetail } from '../services/api';
import { FaPlus, FaEdit, FaSearch } from 'react-icons/fa';
import { toast } from 'react-toastify';

const BankDetailsManagement = () => {
  const [bankDetails, setBankDetails] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [currentBankDetail, setCurrentBankDetail] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [formData, setFormData] = useState({
    employeeId: '',
    accountHolderName: '',
    accountNumber: '',
    bankName: '',
    branchName: '',
    ifscCode: '',
    accountType: 'Savings'
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const employeesData = await getEmployees();
        setEmployees(employeesData);

        // Fetch bank details for all employees
        const bankDetailsPromises = employeesData.map(employee =>
          getBankDetailByEmployee(employee.id)
            .then(data => {
              // Ensure each bank detail has a unique ID
              // If the bank detail doesn't have an ID (which might be causing duplicate keys), 
              // generate a unique one using the employee ID
              if (!data.id || data.id === '00000000-0000-0000-0000-000000000000') {
                data.id = `bank-${employee.id}`;
              }
              return { ...data, employeeName: `${employee.firstName} ${employee.lastName}` };
            })
            .catch(() => null) // Handle case where employee has no bank details
        );

        const bankDetailsResults = await Promise.all(bankDetailsPromises);
        setBankDetails(bankDetailsResults.filter(detail => detail !== null));
      } catch (error) {
        console.error('Error fetching data:', error);
        toast.error('Failed to load bank details');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData({
      ...formData,
      [name]: value
    });
  };

  const openModal = (bankDetail = null) => {
    if (bankDetail) {
      // Edit mode
      setCurrentBankDetail(bankDetail);
      setFormData({
        employeeId: bankDetail.employeeId,
        accountHolderName: bankDetail.accountHolderName,
        accountNumber: bankDetail.accountNumber,
        bankName: bankDetail.bankName,
        branchName: bankDetail.branchName,
        ifscCode: bankDetail.ifscCode,
        accountType: bankDetail.accountType
      });
    } else {
      // Add mode
      setCurrentBankDetail(null);
      setFormData({
        employeeId: '',
        accountHolderName: '',
        accountNumber: '',
        bankName: '',
        branchName: '',
        ifscCode: '',
        accountType: 'Savings'
      });
    }
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      // Prepare the data according to the backend model
      const bankDetailData = {
        ...formData,
        // Make sure field names match the backend model
        employeeId: formData.employeeId,
        accountHolderName: formData.accountHolderName,
        accountNumber: formData.accountNumber,
        bankName: formData.bankName,
        branchName: formData.branchName,
        ifscCode: formData.ifscCode // Note: Backend expects IFSCCode but we're using ifscCode in frontend
      };

      console.log('Submitting bank details:', bankDetailData);

      let result;
      if (currentBankDetail) {
        // Update existing bank detail
        result = await updateBankDetail(currentBankDetail.id, bankDetailData);
        toast.success('Bank details updated successfully');

        // Update the bankDetails state
        setBankDetails(bankDetails.map(bd =>
          bd.id === currentBankDetail.id
            ? { ...result, employeeName: bd.employeeName }
            : bd
        ));
      } else {
        // Create new bank detail
        result = await createBankDetail(bankDetailData);
        toast.success('Bank details added successfully');

        // Find employee name for the new bank detail
        const employee = employees.find(emp => emp.id === formData.employeeId);
        const employeeName = employee
          ? `${employee.firstName} ${employee.lastName}`
          : 'Unknown Employee';

        // Add the new bank detail to the state
        setBankDetails([...bankDetails, { ...result, employeeName }]);
      }

      setShowModal(false);
    } catch (error) {
      console.error('Error saving bank details:', error);
      if (error.response) {
        console.error('Error response:', error.response.data);
        toast.error(`Failed to save bank details: ${error.response.data.message || error.message}`);
      } else {
        toast.error(`Failed to save bank details: ${error.message}`);
      }
    }
  };

  const filteredBankDetails = bankDetails.filter(detail => {
    if (!detail) return false;

    return detail.employeeName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
           detail.bankName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
           detail.accountNumber?.includes(searchTerm);
  });

  const getEmployeeWithoutBankDetails = () => {
    return employees.filter(employee =>
      !bankDetails.some(detail => detail.employeeId === employee.id)
    );
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Bank Details Management</h1>

      <div className="flex justify-between mb-6">
        <div className="relative">
          <input
            type="text"
            placeholder="Search by employee, bank or account"
            className="pl-10 pr-4 py-2 border rounded-lg w-64"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <FaSearch className="absolute left-3 top-3 text-gray-400" />
        </div>
      </div>

      {loading ? (
        <div className="text-center py-10">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
          <p className="mt-3">Loading bank details...</p>
        </div>
      ) : (
        <div className="overflow-x-auto bg-white rounded-lg shadow">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Account Holder</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Account Number</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Bank Name</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Branch</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">IFSC Code</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Account Type</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredBankDetails.length > 0 ? (
                filteredBankDetails.map((detail, index) => (
                  <tr key={`bank-detail-${detail.employeeId || index}`} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.employeeName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.accountHolderName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.accountNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.bankName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.branchName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.ifscCode}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {detail.accountType}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        onClick={() => openModal(detail)}
                        className="text-blue-600 hover:text-blue-900"
                      >
                        <FaEdit className="text-lg" />
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="8" className="px-6 py-4 text-center text-gray-500">
                    No bank details found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Add/Edit Bank Details Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-8 w-full max-w-2xl">
            <h2 className="text-xl font-bold mb-4">
              {currentBankDetail ? 'Edit Bank Details' : 'Add Bank Details'}
            </h2>

            <form onSubmit={handleSubmit}>
              <div className="grid grid-cols-2 gap-4 mb-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Employee
                  </label>
                  <select
                    name="employeeId"
                    value={formData.employeeId}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                    disabled={currentBankDetail !== null}
                  >
                    <option value="">Select Employee</option>
                    {currentBankDetail
                      ? employees
                          .filter(emp => emp.id === currentBankDetail.employeeId)
                          .map(employee => (
                            <option key={employee.id} value={employee.id}>
                              {employee.firstName} {employee.lastName}
                            </option>
                          ))
                      : getEmployeeWithoutBankDetails().map(employee => (
                          <option key={employee.id} value={employee.id}>
                            {employee.firstName} {employee.lastName}
                          </option>
                        ))
                    }
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Account Type
                  </label>
                  <select
                    name="accountType"
                    value={formData.accountType}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  >
                    <option value="Savings">Savings</option>
                    <option value="Current">Current</option>
                    <option value="Salary">Salary</option>
                    <option value="Fixed Deposit">Fixed Deposit</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Account Holder Name
                  </label>
                  <input
                    type="text"
                    name="accountHolderName"
                    value={formData.accountHolderName}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Account Number
                  </label>
                  <input
                    type="text"
                    name="accountNumber"
                    value={formData.accountNumber}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Bank Name
                  </label>
                  <input
                    type="text"
                    name="bankName"
                    value={formData.bankName}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Branch Name
                  </label>
                  <input
                    type="text"
                    name="branchName"
                    value={formData.branchName}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    IFSC Code
                  </label>
                  <input
                    type="text"
                    name="ifscCode"
                    value={formData.ifscCode}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>
              </div>

              <div className="flex justify-end space-x-4 mt-6">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg"
                >
                  {currentBankDetail ? 'Update Bank Details' : 'Save Bank Details'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default BankDetailsManagement;
