import React, { useState, useEffect } from 'react';

const AttendanceStats = ({ stats, loading, dateRange, onDateRangeChange }) => {
  const [animate, setAnimate] = useState(false);
  
  useEffect(() => {
    // Trigger animation when stats load
    if (!loading) {
      setAnimate(true);
    }
  }, [loading]);

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden">
      <div className="px-3 py-3 sm:px-4 border-b border-gray-200">
        <div className="flex flex-wrap items-center justify-between">
          <h3 className="text-base font-medium text-gray-900 mb-2 sm:mb-0">Attendance Stats</h3>
          <div className="w-full sm:w-auto mt-1 sm:mt-0">
            <select 
              className="block w-full pl-2 pr-8 py-1 text-xs border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md"
              value={dateRange}
              onChange={(e) => onDateRangeChange(e.target.value)}
            >
              <option value="7days">Last 7 Days</option>
              <option value="30days">Last 30 Days</option>
              <option value="90days">Last 90 Days</option>
            </select>
          </div>
        </div>
      </div>
      
      {loading ? (
        <div className="p-3 sm:p-4">
          <div className="animate-pulse space-y-3">
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
              {[1, 2, 3, 4].map(i => (
                <div key={i} className="h-16 bg-gray-200 rounded"></div>
              ))}
            </div>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-4 divide-x divide-y md:divide-y-0 divide-gray-200">
          <StatCard 
            title="On-Time %" 
            value={`${stats.onTimePercentage}%`}
            icon={
              <svg className="w-4 h-4 sm:w-5 sm:h-5 text-green-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            }
            animate={animate}
            color="green"
          />
          
          <StatCard 
            title="Total Hours" 
            value={`${stats.totalHours}h`}
            icon={
              <svg className="w-4 h-4 sm:w-5 sm:h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            }
            animate={animate}
            color="blue"
          />
          
          <StatCard 
            title="Avg Hours/Day" 
            value={`${stats.averageHoursPerDay}h`}
            icon={
              <svg className="w-4 h-4 sm:w-5 sm:h-5 text-purple-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            }
            animate={animate}
            color="purple"
          />
          
          <StatCard 
            title="Late Arrivals" 
            value={stats.lateCount}
            icon={
              <svg className="w-4 h-4 sm:w-5 sm:h-5 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            }
            animate={animate}
            color="red"
          />
        </div>
      )}
    </div>
  );
};

const StatCard = ({ title, value, icon, animate, color }) => {
  return (
    <div className={`p-2 sm:p-3 md:p-4 ${animate ? 'animate-fadeIn' : 'opacity-0'}`}>
      <div className="flex items-center mb-1">
        {icon}
        <h4 className="ml-1 text-xs font-medium text-gray-500">{title}</h4>
      </div>
      <div className={`text-lg sm:text-xl md:text-2xl font-bold text-${color}-600 ${animate ? 'animate-scaleIn' : 'opacity-0 scale-50'}`}>
        {value}
      </div>
    </div>
  );
};

export default AttendanceStats;
