import React, { createContext, useContext, useState, useEffect } from 'react';
import { isAuthenticated as checkAuth, getCurrentUser, logout as apiLogout } from '../services/api';

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(checkAuth());
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAuthentication = () => {
      const authenticated = checkAuth();
      const userData = getCurrentUser();
      setIsAuthenticated(authenticated);
      
      // Handle the new user data structure
      if (authenticated && userData) {
        // If userData has a user property, use that, otherwise use userData directly
        setUser(userData.user || userData);
      } else {
        setUser(null);
      }
      
      setLoading(false);
    };

    // Initial check
    checkAuthentication();

    // Listen for storage events (when localStorage changes)
    const handleStorageChange = () => {
      checkAuthentication();
    };
    
    // Custom event for login/logout
    const handleAuthChange = () => {
      checkAuthentication();
    };
    
    window.addEventListener('storage', handleStorageChange);
    window.addEventListener('auth-change', handleAuthChange);
    
    return () => {
      window.removeEventListener('storage', handleStorageChange);
      window.removeEventListener('auth-change', handleAuthChange);
    };
  }, []);

  const logout = () => {
    apiLogout();
    setUser(null);
    setIsAuthenticated(false);
    window.dispatchEvent(new Event('auth-change'));
  };

  const value = {
    user,
    loading,
    isAuthenticated,
    logout
  };

  return (
    <AuthContext.Provider value={value}>
      {!loading && children}
    </AuthContext.Provider>
  );
};

// Don't export default to avoid Fast Refresh issues
// export default AuthContext;
