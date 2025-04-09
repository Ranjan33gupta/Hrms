import React from 'react';

const PayrollForm = ({ payroll, setPayroll, errors = {} }) => {
  const handleChange = (e) => {
    const { name, value } = e.target;
    const numericValue = name === 'basicSalary' || name === 'hra' || name === 'allowances' || name === 'deductions'
      ? parseFloat(value) || 0
      : value;

    // Create a copy of the current payroll state
    const updatedPayroll = { ...payroll };

    // Update the specific field
    updatedPayroll[name] = numericValue;

    // Auto-calculate net salary for display purposes only
    if (name === 'basicSalary' || name === 'hra' || name === 'allowances' || name === 'deductions') {
      const basicSalary = updatedPayroll.basicSalary || 0;
      const hra = updatedPayroll.hra || 0;
      const allowances = updatedPayroll.allowances || 0;
      const deductions = updatedPayroll.deductions || 0;

      // Calculate net salary (for display only - not sent to backend)
      updatedPayroll.netSalary = basicSalary + hra + allowances - deductions;
    }

    // Update the state with the new values
    setPayroll(updatedPayroll);

    console.log('Updated payroll:', updatedPayroll);
  };

  return (
    <div className="bg-white rounded-lg shadow-md p-6 mb-6">
      <h2 className="text-xl font-semibold mb-4">Salary Information</h2>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Basic Salary*
          </label>
          <div className="relative">
            <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">₹</span>
            <input
              type="number"
              name="basicSalary"
              value={payroll?.basicSalary || ''}
              onChange={handleChange}
              className={`w-full p-2 pl-8 border rounded-md ${errors.basicSalary ? 'border-red-500' : 'border-gray-300'}`}
              placeholder="Enter basic salary"
              min="0"
              step="0.01"
            />
          </div>
          {errors.basicSalary && <p className="text-red-500 text-xs mt-1">{errors.basicSalary}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            HRA (House Rent Allowance)
          </label>
          <div className="relative">
            <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">₹</span>
            <input
              type="number"
              name="hra"
              value={payroll?.hra || ''}
              onChange={handleChange}
              className="w-full p-2 pl-8 border border-gray-300 rounded-md"
              placeholder="Enter HRA"
              min="0"
              step="0.01"
            />
          </div>
          <p className="text-xs text-gray-500 mt-1">This will be sent as HRA to the backend</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Other Allowances
          </label>
          <div className="relative">
            <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">₹</span>
            <input
              type="number"
              name="allowances"
              value={payroll?.allowances || ''}
              onChange={handleChange}
              className="w-full p-2 pl-8 border border-gray-300 rounded-md"
              placeholder="Enter allowances"
              min="0"
              step="0.01"
            />
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Deductions
          </label>
          <div className="relative">
            <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">₹</span>
            <input
              type="number"
              name="deductions"
              value={payroll?.deductions || ''}
              onChange={handleChange}
              className="w-full p-2 pl-8 border border-gray-300 rounded-md"
              placeholder="Enter deductions"
              min="0"
              step="0.01"
            />
          </div>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Net Salary (Calculated)
          </label>
          <div className="relative">
            <span className="absolute inset-y-0 left-0 flex items-center pl-3 text-gray-500">₹</span>
            <input
              type="number"
              name="netSalary"
              value={payroll?.netSalary || ''}
              className="w-full p-2 pl-8 border border-gray-300 rounded-md bg-gray-50"
              readOnly
            />
          </div>
          <p className="text-xs text-gray-500 mt-1">Calculated on the backend (display only)</p>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Salary Month
          </label>
          <input
            type="month"
            name="salaryMonth"
            value={payroll?.salaryMonth ? (typeof payroll.salaryMonth === 'string' ? payroll.salaryMonth.substring(0, 7) : new Date(payroll.salaryMonth).toISOString().substring(0, 7)) : ''}
            onChange={handleChange}
            className="w-full p-2 border border-gray-300 rounded-md"
          />
        </div>
      </div>
    </div>
  );
};

export default PayrollForm;
