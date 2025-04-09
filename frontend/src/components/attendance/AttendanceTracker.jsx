import React, { useState, useEffect } from 'react';
import { getTodayAttendance, clockIn, clockOut, getLocationData, getCurrentShift } from '../../services/attendanceService';

const AttendanceTracker = ({ employeeId }) => {
  const [attendance, setAttendance] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [processing, setProcessing] = useState(false);
  const [shift, setShift] = useState(null);
  const [isClockedIn, setIsClockedIn] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        console.log('Fetching attendance data for employee ID:', employeeId);
        
        if (!employeeId) {
          console.log('No employee ID available yet, skipping fetch');
          setLoading(false);
          return;
        }
        
        const [attendanceData, shiftData] = await Promise.all([
          getTodayAttendance(employeeId),
          getCurrentShift(employeeId)
        ]);
        console.log('Attendance data fetched:', attendanceData);
        console.log('Shift data fetched:', shiftData);
        
        setAttendance(attendanceData);
        setShift(shiftData);
        
        if (attendanceData && attendanceData.clockIn && !attendanceData.clockOut) {
          setIsClockedIn(true);
        } else {
          setIsClockedIn(false);
        }
      } catch (err) {
        setError('Unable to connect to attendance service. Please try again later.');
        console.error('Error fetching attendance data:', err);
      } finally {
        setLoading(false);
      }
    };

    if (employeeId) {
      fetchData();
    } else {
      setLoading(false);
    }
  }, [employeeId]);

  const handleClockIn = async () => {
    if (!employeeId || processing || isClockedIn) return;

    try {
      setProcessing(true);
      const location = await getLocationData();
      
      const result = await clockIn(employeeId, location);
      setAttendance(result);
      setIsClockedIn(true);
      
      alert('Clock-in successful with location data captured!');
    } catch (err) {
      setError(err.message || 'Failed to clock in');
      alert(`Error clocking in: ${err.message || 'Please try again'}`);
      console.error('Error clocking in:', err);
    } finally {
      setProcessing(false);
    }
  };

  const handleClockOut = async () => {
    if (!employeeId || processing || !isClockedIn || !attendance) return;

    try {
      setProcessing(true);
      const location = await getLocationData();
      
      const result = await clockOut(employeeId, location);
      setAttendance(result);
      setIsClockedIn(false);
      
      alert('Clock-out successful with location data captured!');
    } catch (err) {
      setError(err.message || 'Failed to clock out');
      alert(`Error clocking out: ${err.message || 'Please try again'}`);
      console.error('Error clocking out:', err);
    } finally {
      setProcessing(false);
    }
  };

  const formatTime = (timeSpan) => {
    if (!timeSpan) return 'N/A';
    
    const parts = timeSpan.split(':');
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);
    
    const ampm = hours >= 12 ? 'PM' : 'AM';
    const hour12 = hours % 12 || 12;
    return `${hour12}:${minutes.toString().padStart(2, '0')} ${ampm}`;
  };

  const renderShiftInfo = () => {
    if (!shift) return null;
    
    return (
      <div className="mt-2 text-sm text-gray-600">
        <p>Current Shift: {shift.shiftName}</p>
        <p>Hours: {formatTime(shift.shiftStartTime)} - {formatTime(shift.shiftEndTime)}</p>
      </div>
    );
  };

  if (loading) {
    return (
      <div className="p-4 bg-white rounded-lg shadow-md">
        <div className="animate-pulse flex space-x-4">
          <div className="flex-1 space-y-4 py-1">
            <div className="h-4 bg-gray-200 rounded w-3/4"></div>
            <div className="space-y-2">
              <div className="h-4 bg-gray-200 rounded"></div>
              <div className="h-4 bg-gray-200 rounded w-5/6"></div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="p-4 bg-white rounded-lg shadow-md">
      <h3 className="text-lg font-semibold text-gray-800 mb-4">Attendance Tracker</h3>
      
      {error && (
        <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-2 mb-4" role="alert">
          <p>{error}</p>
        </div>
      )}
      
      <div className="flex flex-col space-y-4">
        {attendance ? (
          <div>
            <div className="grid grid-cols-2 gap-4 mb-4">
              <div>
                <span className="text-sm text-gray-600">Clock In</span>
                <p className="font-semibold">{formatTime(attendance.clockIn)}</p>
                <p className="text-xs text-gray-500 truncate" title={attendance.checkInLocation}>
                  {attendance.checkInLocation || 'Location not available'}
                </p>
              </div>
              
              {attendance.clockOut ? (
                <div>
                  <span className="text-sm text-gray-600">Clock Out</span>
                  <p className="font-semibold">{formatTime(attendance.clockOut)}</p>
                  <p className="text-xs text-gray-500 truncate" title={attendance.checkOutLocation}>
                    {attendance.checkOutLocation || 'Location not available'}
                  </p>
                </div>
              ) : (
                <div>
                  <span className="text-sm text-gray-600">Clock Out</span>
                  <p className="font-semibold">--:-- --</p>
                  <p className="text-xs text-gray-500">Not clocked out yet</p>
                </div>
              )}
            </div>
            
            {isClockedIn && !attendance.clockOut && (
              <button
                onClick={handleClockOut}
                disabled={processing}
                className="w-full py-2 bg-red-600 hover:bg-red-700 text-white rounded-md transition duration-200 flex items-center justify-center"
              >
                {processing ? (
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : (
                  <>
                    <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M5 13l4 4L19 7"></path>
                    </svg>
                    Clock Out
                  </>
                )}
              </button>
            )}
            
            {renderShiftInfo()}
            
            {attendance.isLate && (
              <div className="mt-2 text-sm text-amber-600">
                <p>* You were late today</p>
              </div>
            )}
          </div>
        ) : (
          <div>
            <p className="text-gray-600 mb-4">You haven't clocked in today.</p>
            
            {renderShiftInfo()}
            
            {!isClockedIn && (
              <button
                onClick={handleClockIn}
                disabled={processing}
                className="w-full py-2 bg-green-600 hover:bg-green-700 text-white rounded-md transition duration-200 flex items-center justify-center"
              >
                {processing ? (
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : (
                  <>
                    <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    Clock In
                  </>
                )}
              </button>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default AttendanceTracker;
