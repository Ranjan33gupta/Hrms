import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getPayrollHistoriesByEmployeeId, getEmployee } from '../../services/api';

const PayrollHistory = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [payrollHistories, setPayrollHistories] = useState([]);
  const [employee, setEmployee] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        
        // Fetch employee details
        const employeeData = await getEmployee(id);
        setEmployee(employeeData);
        
        // Fetch payroll history records
        const payrollData = await getPayrollHistoriesByEmployeeId(id);
        setPayrollHistories(payrollData);
        
        setIsLoading(false);
      } catch (err) {
        console.error('Error fetching payroll history:', err);
        setError('Failed to load payroll history. Please try again later.');
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
    return date.toLocaleDateString();
  };

  const formatCurrency = (amount) => {
    if (amount === undefined || amount === null) return 'N/A';
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR'
    }).format(amount);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <p className="text-gray-600">Loading payroll history...</p>
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

  return (
    <div className="container mx-auto p-4">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Payroll History</h1>
          {employee && (
            <p className="text-gray-600">
              {employee.fullName} ({employee.email})
            </p>
          )}
        </div>
        <button
          onClick={() => navigate(-1)}
          className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-md"
        >
          Back
        </button>
      </div>

      {payrollHistories.length === 0 ? (
        <div className="bg-yellow-50 border-l-4 border-yellow-400 p-4 rounded">
          <p className="text-yellow-700">No payroll history records found for this employee.</p>
        </div>
      ) : (
        <div className="space-y-6">
          {payrollHistories.map((history, index) => {
            // Extract the change details from the payroll history
            const changeDetails = history.payrollChangeDetails || {};
            const changeDate = Object.keys(changeDetails)[0] || '';
            const changes = changeDetails[changeDate] || [];
            
            return (
              <div key={index} className="bg-white shadow-md rounded-lg overflow-hidden">
                <div className="bg-blue-50 px-6 py-4 border-b border-blue-100">
                  <div className="flex justify-between items-center">
                    <h2 className="text-lg font-semibold text-blue-800">
                      Change Date: {formatDate(changeDate)}
                    </h2>
                    <span className="px-3 py-1 text-sm rounded-full bg-blue-100 text-blue-800">
                      {history.createdBy || 'System'}
                    </span>
                  </div>
                </div>
                
                <div className="p-6">
                  <h3 className="text-md font-medium text-gray-700 mb-4">Payroll Changes</h3>
                  
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Component
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Old Value
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          New Value
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {changes.map((change, changeIndex) => (
                        <tr key={changeIndex} className="hover:bg-gray-50">
                          <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                            {change.fieldChanged}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {change.fieldChanged.toLowerCase().includes('salary') || 
                             change.fieldChanged.toLowerCase().includes('amount') ? 
                              formatCurrency(change.oldValue) : change.oldValue}
                          </td>
                          <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                            {change.fieldChanged.toLowerCase().includes('salary') || 
                             change.fieldChanged.toLowerCase().includes('amount') ? 
                              formatCurrency(change.newValue) : change.newValue}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
};

export default PayrollHistory;
