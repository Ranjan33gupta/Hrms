import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

// Protected route component that checks if user is authenticated
// and optionally if they have the required role
const ProtectedRoute = ({ children, requiredRole }) => {
  const { user, isAuthenticated } = useAuth();
  
  // If not authenticated, redirect to login
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }
  
  // If role is required and user doesn't have it, redirect to appropriate dashboard
  if (requiredRole && user?.role !== requiredRole) {
    // If user is an employee, redirect to employee dashboard
    if (user?.role === 'Employee') {
      return <Navigate to="/employee-dashboard" replace />;
    }
    // If user is an admin, redirect to admin dashboard
    if (user?.role === 'Admin') {
      return <Navigate to="/dashboard" replace />;
    }
    // Default fallback
    return <Navigate to="/" replace />;
  }
  
  // If authenticated and has required role (if any), render the children
  return children;
};

export default ProtectedRoute;
