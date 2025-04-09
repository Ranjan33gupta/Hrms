import React from 'react';
import { useParams } from 'react-router-dom';
import EmployeeDashboard from '../components/employee/EmployeeDashboard';
import EmployeeDetailView from '../components/employee/EmployeeDetailView';

const EmployeePage = () => {
  const { id } = useParams();

  // If an ID is provided, show the employee detail view
  // Otherwise, show the employee dashboard (for the logged-in employee)
  return (
    <div>
      {id ? <EmployeeDetailView /> : <EmployeeDashboard />}
    </div>
  );
};

export default EmployeePage;
