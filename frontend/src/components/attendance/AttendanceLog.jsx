import React, { useState } from 'react';

const AttendanceLog = ({ attendanceData, loading }) => {
  const [filter, setFilter] = useState('all');

  // Filter logs based on selected filter
  const filteredLogs = filter === 'all' 
    ? attendanceData
    : filter === 'late' 
      ? attendanceData.filter(record => record.isLate) 
      : attendanceData.filter(record => !record.isLate);

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'short',
      month: 'short',
      day: 'numeric'
    });
  };

  const formatTime = (timeString) => {
    if (!timeString) return '--:--';
    const date = new Date(timeString);
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit',
      hour12: true 
    });
  };

  return (
    <div className="animate-fadeIn">
      <div className="px-4 py-3 sm:px-6 border-b border-gray-200 bg-gray-50 flex flex-wrap items-center justify-between">
        <div className="w-full sm:w-auto mb-2 sm:mb-0">
          <div className="flex space-x-1">
            <button
              onClick={() => setFilter('all')}
              className={`px-3 py-1.5 text-xs sm:text-sm font-medium rounded-md ${
                filter === 'all' 
                  ? 'bg-indigo-100 text-indigo-700' 
                  : 'text-gray-500 hover:text-gray-700 bg-white'
              }`}
            >
              All
            </button>
            <button
              onClick={() => setFilter('ontime')}
              className={`px-3 py-1.5 text-xs sm:text-sm font-medium rounded-md ${
                filter === 'ontime' 
                  ? 'bg-green-100 text-green-700' 
                  : 'text-gray-500 hover:text-gray-700 bg-white'
              }`}
            >
              On Time
            </button>
            <button
              onClick={() => setFilter('late')}
              className={`px-3 py-1.5 text-xs sm:text-sm font-medium rounded-md ${
                filter === 'late' 
                  ? 'bg-red-100 text-red-700' 
                  : 'text-gray-500 hover:text-gray-700 bg-white'
              }`}
            >
              Late
            </button>
          </div>
        </div>
        <div className="w-full sm:w-auto flex items-center">
          <span className="text-xs text-gray-500 mr-2">Showing {filteredLogs.length} of {attendanceData.length}</span>
          <select className="block w-full sm:w-auto pl-3 pr-8 py-1.5 text-xs border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 rounded-md">
            <option>Recent First</option>
            <option>Oldest First</option>
          </select>
        </div>
      </div>

      {loading ? (
        <div className="p-4">
          <div className="animate-pulse space-y-4">
            {[1, 2, 3, 4, 5].map(i => (
              <div key={i} className="h-16 bg-gray-100 rounded"></div>
            ))}
          </div>
        </div>
      ) : filteredLogs.length === 0 ? (
        <div className="text-center py-8">
          <svg className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <h3 className="mt-2 text-sm font-medium text-gray-900">No attendance records found</h3>
          <p className="mt-1 text-sm text-gray-500">
            No attendance records match your current filter.
          </p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th scope="col" className="px-3 sm:px-6 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Date
                </th>
                <th scope="col" className="px-3 sm:px-6 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th scope="col" className="px-3 sm:px-6 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Clock In
                </th>
                <th scope="col" className="px-3 sm:px-6 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Clock Out
                </th>
                <th scope="col" className="px-3 sm:px-6 py-2 sm:py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Hours
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {filteredLogs.map((record, index) => (
                <tr 
                  key={index} 
                  className={`${record.isLate ? 'bg-red-50' : ''} hover:bg-gray-50 transition-colors`}
                  style={{ animationDelay: `${index * 50}ms` }}
                >
                  <td className="px-3 sm:px-6 py-2 sm:py-4 whitespace-nowrap text-xs sm:text-sm text-gray-900">
                    {formatDate(record.date)}
                  </td>
                  <td className="px-3 sm:px-6 py-2 sm:py-4 whitespace-nowrap">
                    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                      record.isLate ? 'bg-red-100 text-red-800' : 'bg-green-100 text-green-800'
                    }`}>
                      {record.isLate ? 'Late' : 'On Time'}
                    </span>
                  </td>
                  <td className="px-3 sm:px-6 py-2 sm:py-4 whitespace-nowrap text-xs sm:text-sm text-gray-900">
                    {formatTime(record.clockIn)}
                  </td>
                  <td className="px-3 sm:px-6 py-2 sm:py-4 whitespace-nowrap text-xs sm:text-sm text-gray-900">
                    {formatTime(record.clockOut)}
                  </td>
                  <td className="px-3 sm:px-6 py-2 sm:py-4 whitespace-nowrap text-xs sm:text-sm text-gray-900">
                    {record.hoursWorked ? `${record.hoursWorked.toFixed(1)}h` : '--'}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default AttendanceLog;
