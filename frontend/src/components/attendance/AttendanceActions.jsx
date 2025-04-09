import React, { useState } from 'react';
import { getLocationData, clockIn, clockOut } from '../../services/attendanceService';
import { useAuth } from '../../contexts/AuthContext';

const AttendanceActions = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(false);
  const [success, setSuccess] = useState(null);
  const [error, setError] = useState(null);
  const [showActions, setShowActions] = useState(false);

  // Format current time
  const getCurrentTime = () => {
    const now = new Date();
    return now.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    });
  };

  const getCurrentDate = () => {
    const now = new Date();
    return now.toLocaleDateString('en-US', { 
      weekday: 'short',
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  };

  const handleClockIn = async () => {
    if (!user?.employeeId) {
      setError('User information not available');
      return;
    }

    setLoading(true);
    setError(null);
    setSuccess(null);
    
    try {
      // First try to get location data
      let location;
      try {
        location = await getLocationData();
        console.log('Location data for clock in:', location);
      } catch (locationErr) {
        console.error('Location error during clock in:', locationErr);
        // Continue with default location data
        location = {
          latitude: 0,
          longitude: 0,
          address: 'Location not available',
          timestamp: new Date().toISOString()
        };
      }
      
      // Proceed with clock in regardless of location success
      const result = await clockIn(user.employeeId, location);
      
      setSuccess(`Successfully clocked in at ${getCurrentTime()}`);
      setShowActions(false);
      
      // Refresh the page after 2 seconds
      setTimeout(() => {
        window.location.reload();
      }, 2000);
    } catch (err) {
      console.error('Clock in error:', err);
      setError(err.message || 'Failed to clock in. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleClockOut = async () => {
    if (!user?.employeeId) {
      setError('User information not available');
      return;
    }

    setLoading(true);
    setError(null);
    setSuccess(null);
    
    try {
      // First try to get location data
      let location;
      try {
        location = await getLocationData();
        console.log('Location data for clock out:', location);
      } catch (locationErr) {
        console.error('Location error during clock out:', locationErr);
        // Continue with default location data
        location = {
          latitude: 0,
          longitude: 0,
          address: 'Location not available',
          timestamp: new Date().toISOString()
        };
      }
      
      // Proceed with clock out regardless of location success
      const result = await clockOut(user.employeeId, location);
      
      setSuccess(`Successfully clocked out at ${getCurrentTime()}`);
      setShowActions(false);
      
      // Refresh the page after 2 seconds
      setTimeout(() => {
        window.location.reload();
      }, 2000);
    } catch (err) {
      console.error('Clock out error:', err);
      setError(err.message || 'Failed to clock out. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden animate-fadeIn">
      <div className="px-3 py-3 sm:px-4 sm:py-4 border-b border-gray-200">
        <h3 className="text-base sm:text-lg font-medium text-gray-900">Actions</h3>
      </div>
      
      <div className="p-3 sm:p-6">
        <div className="flex flex-col items-center">
          <div className="rounded-full bg-indigo-100 p-2 sm:p-3 mb-2 sm:mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 text-indigo-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          
          <div className="text-center mb-4">
            <h3 className="text-lg sm:text-xl font-semibold text-gray-800">{getCurrentTime()}</h3>
            <p className="text-xs sm:text-sm text-gray-500">{getCurrentDate()}</p>
          </div>
          
          {error && (
            <div className="mb-3 w-full rounded-md bg-red-50 p-2 sm:p-3">
              <div className="flex">
                <div className="flex-shrink-0">
                  <svg className="h-4 w-4 text-red-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-2">
                  <p className="text-xs sm:text-sm text-red-700">{error}</p>
                </div>
              </div>
            </div>
          )}
          
          {success && (
            <div className="mb-3 w-full rounded-md bg-green-50 p-2 sm:p-3">
              <div className="flex">
                <div className="flex-shrink-0">
                  <svg className="h-4 w-4 text-green-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-2">
                  <p className="text-xs sm:text-sm text-green-700">{success}</p>
                </div>
              </div>
            </div>
          )}
          
          {!showActions ? (
            <button
              className="w-full py-2 sm:py-3 px-4 rounded-md bg-indigo-600 text-white text-sm sm:text-base font-medium focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 hover:bg-indigo-700 transition-all transform hover:scale-105 active:scale-95"
              onClick={() => setShowActions(true)}
              disabled={loading}
            >
              {loading ? (
                <svg className="animate-spin h-5 w-5 mx-auto text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              ) : (
                <>Attendance Actions</>
              )}
            </button>
          ) : (
            <div className="space-y-2 sm:space-y-3 w-full">
              <button
                className="w-full py-2 sm:py-3 px-4 rounded-md bg-green-600 text-white text-sm sm:text-base font-medium flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500 hover:bg-green-700 transition-all transform hover:scale-105 active:scale-95"
                onClick={handleClockIn}
                disabled={loading}
              >
                {loading ? (
                  <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : (
                  <>
                    <svg className="w-4 h-4 sm:w-5 sm:h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
                    </svg>
                    Clock In
                  </>
                )}
              </button>
              
              <button
                className="w-full py-2 sm:py-3 px-4 rounded-md bg-red-600 text-white text-sm sm:text-base font-medium flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500 hover:bg-red-700 transition-all transform hover:scale-105 active:scale-95"
                onClick={handleClockOut}
                disabled={loading}
              >
                {loading ? (
                  <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : (
                  <>
                    <svg className="w-4 h-4 sm:w-5 sm:h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    Clock Out
                  </>
                )}
              </button>
              
              <button
                className="w-full py-2 sm:py-3 px-4 rounded-md bg-gray-200 text-gray-800 text-sm sm:text-base font-medium flex items-center justify-center focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500 hover:bg-gray-300 transition-all transform hover:scale-105 active:scale-95"
                onClick={() => setShowActions(false)}
              >
                Cancel
              </button>
            </div>
          )}
        </div>
        
        <div className="mt-4 border-t border-gray-200 pt-3">
          <h4 className="text-xs sm:text-sm font-medium text-gray-900 mb-2">Quick Links</h4>
          
          <ul className="space-y-2">
            <li>
              <a 
                href="#" 
                className="flex items-center text-xs sm:text-sm text-indigo-600 hover:text-indigo-800"
              >
                <svg className="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
                </svg>
                View Attendance Policy
              </a>
            </li>
            <li>
              <a 
                href="#" 
                className="flex items-center text-xs sm:text-sm text-indigo-600 hover:text-indigo-800"
              >
                <svg className="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
                Request Time Off
              </a>
            </li>
            <li>
              <a 
                href="#" 
                className="flex items-center text-xs sm:text-sm text-indigo-600 hover:text-indigo-800"
              >
                <svg className="w-3 h-3 sm:w-4 sm:h-4 mr-1 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                Work From Home
              </a>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
};

export default AttendanceActions;
