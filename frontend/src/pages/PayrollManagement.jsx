import React, { useState, useEffect } from 'react';
import { getPayrolls, getEmployees, createPayroll, updatePayroll } from '../services/api';
import { FaPlus, FaEdit, FaSearch } from 'react-icons/fa';
import { toast } from 'react-toastify';

const PayrollManagement = () => {
  const [payrolls, setPayrolls] = useState([]);
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [currentPayroll, setCurrentPayroll] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [formData, setFormData] = useState({
    employeeId: '',
    basicSalary: 0,
    hra: 0, // Added HRA field to match backend model
    allowances: 0,
    deductions: 0,
    salaryMonth: new Date().toISOString().split('T')[0], // Changed to match backend model
    paymentDate: new Date().toISOString().split('T')[0],
    paymentStatus: 'Pending'
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const [payrollsData, employeesData] = await Promise.all([
          getPayrolls(),
          getEmployees()
        ]);
        setPayrolls(payrollsData);
        setEmployees(employeesData);
      } catch (error) {
        console.error('Error fetching data:', error);
        toast.error('Failed to load payroll data');
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
      [name]: name === 'basicSalary' || name === 'allowances' || name === 'deductions'
        ? parseFloat(value) || 0
        : value
    });
  };

  const openModal = (payroll = null) => {
    if (payroll) {
      // Edit mode
      setCurrentPayroll(payroll);
      setFormData({
        employeeId: payroll.employeeId,
        basicSalary: payroll.basicSalary,
        hra: payroll.hra || 0, // Added HRA field
        allowances: payroll.allowances,
        deductions: payroll.deductions,
        salaryMonth: payroll.salaryMonth ? payroll.salaryMonth.split('T')[0] : new Date().toISOString().split('T')[0],
        paymentDate: payroll.paymentDate ? payroll.paymentDate.split('T')[0] : new Date().toISOString().split('T')[0],
        paymentStatus: payroll.paymentStatus || 'Pending'
      });
    } else {
      // Add mode
      setCurrentPayroll(null);
      setFormData({
        employeeId: '',
        basicSalary: 0,
        hra: 0,
        allowances: 0,
        deductions: 0,
        salaryMonth: new Date().toISOString().split('T')[0],
        paymentDate: new Date().toISOString().split('T')[0],
        paymentStatus: 'Pending'
      });
    }
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      // Prepare the data according to the backend model
      const payrollData = {
        employeeId: formData.employeeId,
        basicSalary: parseFloat(formData.basicSalary),
        hra: parseFloat(formData.hra),
        allowances: parseFloat(formData.allowances),
        deductions: parseFloat(formData.deductions),
        // Calculate net salary
        netSalary: parseFloat(formData.basicSalary) + parseFloat(formData.hra) + parseFloat(formData.allowances) - parseFloat(formData.deductions),
        salaryMonth: new Date(formData.salaryMonth).toISOString(),
        paymentDate: new Date(formData.paymentDate).toISOString()
      };

      console.log('Submitting payroll data:', payrollData);

      let result;
      if (currentPayroll) {
        // Update existing payroll
        result = await updatePayroll(currentPayroll.id, payrollData);
        toast.success('Payroll updated successfully');

        // Update the payrolls state
        setPayrolls(payrolls.map(p => p.id === currentPayroll.id ? result : p));
      } else {
        // Create new payroll
        result = await createPayroll(payrollData);
        toast.success('Payroll created successfully');

        // Add the new payroll to the state
        setPayrolls([...payrolls, result]);
      }

      setShowModal(false);
    } catch (error) {
      console.error('Error saving payroll:', error);
      if (error.response) {
        console.error('Error response:', error.response.data);
        toast.error(`Failed to save payroll: ${error.response.data.message || error.message}`);
      } else {
        toast.error(`Failed to save payroll: ${error.message}`);
      }
    }
  };

  const filteredPayrolls = payrolls.filter(payroll => {
    const employee = employees.find(emp => emp.id === payroll.employeeId);
    if (!employee) return false;

    const fullName = `${employee.firstName} ${employee.lastName}`.toLowerCase();
    return fullName.includes(searchTerm.toLowerCase()) ||
           payroll.paymentStatus.toLowerCase().includes(searchTerm.toLowerCase());
  });

  const calculateNetSalary = (payroll) => {
    return payroll.basicSalary + payroll.allowances - payroll.deductions;
  };

  const getEmployeeName = (employeeId) => {
    const employee = employees.find(emp => emp.id === employeeId);
    return employee ? `${employee.firstName} ${employee.lastName}` : 'Unknown Employee';
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Payroll Management</h1>

      <div className="flex justify-between mb-6">
        <div className="relative">
          <input
            type="text"
            placeholder="Search by employee name or status"
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
          <p className="mt-3">Loading payroll data...</p>
        </div>
      ) : (
        <div className="overflow-x-auto bg-white rounded-lg shadow">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Pay Period</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Basic Salary</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Allowances</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Deductions</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Net Salary</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredPayrolls.length > 0 ? (
                filteredPayrolls.map((payroll) => (
                  <tr key={payroll.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      {getEmployeeName(payroll.employeeId)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {new Date(payroll.salaryMonth).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      ${payroll.basicSalary.toFixed(2)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      ${payroll.allowances.toFixed(2)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      ${payroll.deductions.toFixed(2)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap font-medium">
                      ${calculateNetSalary(payroll).toFixed(2)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 inline-flex text-xs leading-5 font-semibold rounded-full
                        ${payroll.paymentStatus === 'Paid' ? 'bg-green-100 text-green-800' :
                          payroll.paymentStatus === 'Pending' ? 'bg-yellow-100 text-yellow-800' :
                          'bg-red-100 text-red-800'}`}>
                        {payroll.paymentStatus}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <button
                        onClick={() => openModal(payroll)}
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
                    No payroll records found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Add/Edit Payroll Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-8 w-full max-w-2xl">
            <h2 className="text-xl font-bold mb-4">
              {currentPayroll ? 'Edit Payroll' : 'Add New Payroll'}
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
                  >
                    <option value="">Select Employee</option>
                    {employees.map((employee) => (
                      <option key={employee.id} value={employee.id}>
                        {employee.firstName} {employee.lastName}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Payment Status
                  </label>
                  <select
                    name="paymentStatus"
                    value={formData.paymentStatus}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  >
                    <option value="Pending">Pending</option>
                    <option value="Paid">Paid</option>
                    <option value="Failed">Failed</option>
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Basic Salary
                  </label>
                  <input
                    type="number"
                    name="basicSalary"
                    value={formData.basicSalary}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    min="0"
                    step="0.01"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    HRA
                  </label>
                  <input
                    type="number"
                    name="hra"
                    value={formData.hra}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    min="0"
                    step="0.01"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Allowances
                  </label>
                  <input
                    type="number"
                    name="allowances"
                    value={formData.allowances}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    min="0"
                    step="0.01"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Deductions
                  </label>
                  <input
                    type="number"
                    name="deductions"
                    value={formData.deductions}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    min="0"
                    step="0.01"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Net Salary
                  </label>
                  <input
                    type="text"
                    value={`$${(parseFloat(formData.basicSalary) + parseFloat(formData.hra) + parseFloat(formData.allowances) - parseFloat(formData.deductions)).toFixed(2)}`}
                    className="w-full border rounded-lg px-3 py-2 bg-gray-100"
                    disabled
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Salary Month
                  </label>
                  <input
                    type="date"
                    name="salaryMonth"
                    value={formData.salaryMonth}
                    onChange={handleInputChange}
                    className="w-full border rounded-lg px-3 py-2"
                    required
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    Payment Date
                  </label>
                  <input
                    type="date"
                    name="paymentDate"
                    value={formData.paymentDate}
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
                  {currentPayroll ? 'Update Payroll' : 'Create Payroll'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default PayrollManagement;
