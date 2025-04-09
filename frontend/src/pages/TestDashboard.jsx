import React, { useState, useEffect } from 'react';
import MoodChangerButton from '../components/MoodChanger/MoodChangerButton';

const TestDashboard = () => {
  const [authStatus, setAuthStatus] = useState({
    isAuthenticated: false,
    token: null,
    user: null
  });

  useEffect(() => {
    // Check authentication status
    const token = localStorage.getItem('token');
    const userStr = localStorage.getItem('user');
    
    try {
      const user = userStr ? JSON.parse(userStr) : null;
      setAuthStatus({
        isAuthenticated: !!token,
        token,
        user
      });
      
      console.log('Auth status:', { token: !!token, user });
    } catch (error) {
      console.error('Error parsing user data:', error);
    }
  }, []);

  const handleLogin = () => {
    // Set a test token and user for debugging
    const testToken = 'test-token-' + Date.now();
    const testUser = {
      id: '1',
      firstName: 'Test',
      lastName: 'User',
      email: 'test@example.com',
      role: 'Admin'
    };
    
    localStorage.setItem('token', testToken);
    localStorage.setItem('user', JSON.stringify(testUser));
    
    setAuthStatus({
      isAuthenticated: true,
      token: testToken,
      user: testUser
    });
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
    setAuthStatus({
      isAuthenticated: false,
      token: null,
      user: null
    });
  };

  return (
    <div className="min-h-screen bg-gray-50 p-8">
      <div className="max-w-6xl mx-auto">
        <div className="bg-white rounded-lg shadow-md p-6 mb-6">
          <h1 className="text-3xl font-bold text-gray-800 mb-6">WorkNest Dashboard Diagnostics</h1>
          
          <div className="mb-8 p-4 bg-blue-50 rounded-lg">
            <h2 className="text-xl font-semibold mb-4">Authentication Status</h2>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <p className="text-gray-600">Status:</p>
                <p className={`font-medium ${authStatus.isAuthenticated ? 'text-green-600' : 'text-red-600'}`}>
                  {authStatus.isAuthenticated ? 'Authenticated' : 'Not Authenticated'}
                </p>
              </div>
              {authStatus.user && (
                <div>
                  <p className="text-gray-600">User:</p>
                  <p className="font-medium">{authStatus.user.firstName} {authStatus.user.lastName} ({authStatus.user.role})</p>
                </div>
              )}
            </div>
            
            <div className="mt-4 flex gap-4">
              {!authStatus.isAuthenticated ? (
                <button 
                  onClick={handleLogin}
                  className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition"
                >
                  Set Test Authentication
                </button>
              ) : (
                <button 
                  onClick={handleLogout}
                  className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition"
                >
                  Clear Authentication
                </button>
              )}
            </div>
          </div>
          
          <div className="mb-8">
            <h2 className="text-xl font-semibold mb-4">Navigation Links</h2>
            <div className="flex flex-wrap gap-4">
              <a 
                href="/" 
                className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition"
              >
                Home
              </a>
              <a 
                href="/login" 
                className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition"
              >
                Login
              </a>
              <a 
                href="/dashboard" 
                className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition"
              >
                Dashboard
              </a>
              <a 
                href="/employee-dashboard" 
                className="px-4 py-2 bg-gray-200 text-gray-800 rounded hover:bg-gray-300 transition"
              >
                Employee Dashboard
              </a>
            </div>
          </div>
          
          <div className="mb-8">
            <h2 className="text-xl font-semibold mb-4">API Connection Test</h2>
            <button 
              onClick={() => {
                fetch('http://localhost:5171/api/Auth/Test')
                  .then(response => {
                    console.log('API response:', response);
                    alert(`API Status: ${response.status} ${response.statusText}`);
                  })
                  .catch(error => {
                    console.error('API error:', error);
                    alert(`API Error: ${error.message}`);
                  });
              }}
              className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
            >
              Test API Connection
            </button>
          </div>
          
          <div className="mb-8">
            <h2 className="text-xl font-semibold mb-4">MoodChanger Component Test</h2>
            <div className="p-4 border border-gray-200 rounded-lg">
              <MoodChangerButton />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TestDashboard;
