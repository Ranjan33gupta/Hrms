import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { getAttendanceHistory } from '../services/attendanceService';
import AttendanceStats from '../components/attendance/AttendanceStats';
import AttendanceLog from '../components/attendance/AttendanceLog';
import AttendanceTimings from '../components/attendance/AttendanceTimings';
import AttendanceActions from '../components/attendance/AttendanceActions';

const AttendanceDashboard = () => {
  const { user } = useAuth();
  const [attendanceData, setAttendanceData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('log');
  const [dateRange, setDateRange] = useState('30days');
  const [stats, setStats] = useState({
    onTimePercentage: 0,
    totalHours: 0,
    averageHoursPerDay: 0,
    lateCount: 0
  });

  useEffect(() => {
    const fetchAttendanceData = async () => {
      if (!user?.employeeId) return;
      
      try {
        setLoading(true);
        const data = await getAttendanceHistory(user.employeeId);
        setAttendanceData(data);
        
        // Calculate stats
        if (data && data.length > 0) {
          const totalHours = data.reduce((sum, record) => {
            return sum + (record.hoursWorked || 0);
          }, 0);
          
          const lateCount = data.filter(record => record.isLate).length;
          const onTimeCount = data.length - lateCount;
          
          setStats({
            onTimePercentage: Math.round((onTimeCount / data.length) * 100),
            totalHours: Math.round(totalHours * 10) / 10,
            averageHoursPerDay: Math.round((totalHours / data.length) * 10) / 10,
            lateCount
          });
        }
      } catch (error) {
        console.error('Failed to fetch attendance history:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchAttendanceData();
  }, [user?.employeeId]);

  return (
    <div className="bg-gray-50 min-h-screen">
      <div className="max-w-7xl mx-auto px-3 sm:px-6 lg:px-8 py-3 sm:py-6">
        <header className="mb-4 sm:mb-6">
          <h1 className="text-xl sm:text-2xl md:text-3xl font-bold text-gray-900">Attendance Dashboard</h1>
          <p className="mt-1 text-sm text-gray-600">Track your attendance, check logs, and manage your time</p>
        </header>

        {/* First row - Stats and Actions */}
        <div className="mb-4 sm:mb-6">
          <AttendanceStats stats={stats} loading={loading} dateRange={dateRange} onDateRangeChange={setDateRange} />
        </div>
        
        <div className="mb-4 sm:mb-6">
          <AttendanceActions />
        </div>

        {/* Second row - Timings */}
        <div className="mb-4 sm:mb-6">
          <AttendanceTimings attendanceData={attendanceData} loading={loading} />
        </div>

        {/* Third row - Attendance Log */}
        <div className="bg-white rounded-lg shadow overflow-hidden mb-4 sm:mb-6">
          <div className="border-b border-gray-200 overflow-x-auto">
            <nav className="flex">
              <button
                className={`${
                  activeTab === 'log'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                } whitespace-nowrap py-3 px-3 sm:px-6 border-b-2 font-medium text-xs sm:text-sm`}
                onClick={() => setActiveTab('log')}
              >
                Attendance Log
              </button>
              <button
                className={`${
                  activeTab === 'calendar'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                } whitespace-nowrap py-3 px-3 sm:px-6 border-b-2 font-medium text-xs sm:text-sm`}
                onClick={() => setActiveTab('calendar')}
              >
                Calendar View
              </button>
              <button
                className={`${
                  activeTab === 'requests'
                    ? 'border-indigo-500 text-indigo-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                } whitespace-nowrap py-3 px-3 sm:px-6 border-b-2 font-medium text-xs sm:text-sm`}
                onClick={() => setActiveTab('requests')}
              >
                Requests
              </button>
            </nav>
          </div>
          <div>
            {activeTab === 'log' && <AttendanceLog attendanceData={attendanceData} loading={loading} />}
            {activeTab === 'calendar' && <div className="p-8 text-center text-gray-500">Calendar view coming soon</div>}
            {activeTab === 'requests' && <div className="p-8 text-center text-gray-500">Requests view coming soon</div>}
          </div>
        </div>
      </div>
    </div>
  );
};

export default AttendanceDashboard;
