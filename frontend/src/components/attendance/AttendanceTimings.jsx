import React, { useState, useEffect } from 'react';

const AttendanceTimings = ({ attendanceData, loading }) => {
  const [animate, setAnimate] = useState(false);

  useEffect(() => {
    if (!loading) {
      setAnimate(true);
    }
  }, [loading]);

  // Get data for the current week
  const today = new Date();
  const dayOfWeek = today.getDay();
  const startOfWeek = new Date(today);
  startOfWeek.setDate(today.getDate() - dayOfWeek);
  
  // Create array for each day of current week
  const weekDays = [];
  for (let i = 0; i < 7; i++) {
    const day = new Date(startOfWeek);
    day.setDate(startOfWeek.getDate() + i);
    weekDays.push(day);
  }

  // Format day name
  const formatDayName = (date) => {
    return date.toLocaleDateString('en-US', { weekday: 'short' });
  };

  // Format date as MM/DD
  const formatDate = (date) => {
    return date.toLocaleDateString('en-US', { month: 'numeric', day: 'numeric' });
  };

  // Map attendance data to days of the week
  const weekData = weekDays.map(day => {
    const matchingRecord = attendanceData.find(record => {
      const recordDate = new Date(record.date);
      return recordDate.getDate() === day.getDate() && 
             recordDate.getMonth() === day.getMonth() &&
             recordDate.getFullYear() === day.getFullYear();
    });
    
    return {
      date: day,
      dayName: formatDayName(day),
      formattedDate: formatDate(day),
      isToday: day.toDateString() === today.toDateString(),
      record: matchingRecord,
      hours: matchingRecord ? (matchingRecord.hoursWorked || 0) : 0
    };
  });

  // Calculate max hours for scaling
  const maxHours = Math.max(9, ...weekData.map(day => day.hours));

  return (
    <div className="bg-white rounded-lg shadow overflow-hidden animate-fadeIn">
      <div className="px-3 py-3 sm:px-4 border-b border-gray-200">
        <h3 className="text-base font-medium text-gray-900">Weekly Time Log</h3>
      </div>
      
      {loading ? (
        <div className="p-3 sm:p-4">
          <div className="animate-pulse space-y-3">
            {[1, 2, 3, 4, 5, 6, 7].map(i => (
              <div key={i} className="h-8 bg-gray-100 rounded"></div>
            ))}
          </div>
        </div>
      ) : (
        <div className="p-3 sm:p-4">
          {weekData.map((day, index) => (
            <div 
              key={day.formattedDate}
              className={`mb-2 ${day.isToday ? 'bg-blue-50 rounded p-2' : ''} animate-fadeIn`} 
              style={{ animationDelay: `${index * 100}ms` }}
            >
              <div className="flex items-center">
                <div className="w-12 sm:w-16 flex flex-col">
                  <span className={`text-xs font-medium ${day.isToday ? 'text-blue-600' : 'text-gray-500'}`}>
                    {day.dayName}
                  </span>
                  <span className={`text-xs ${day.isToday ? 'text-blue-600 font-medium' : 'text-gray-400'}`}>
                    {day.formattedDate}
                  </span>
                </div>
                
                <div className="flex-1 ml-2">
                  <div className="flex justify-between text-xs text-gray-500 mb-1">
                    <div>
                      {day.record ? (
                        <>
                          {day.record.clockIn ? new Date(day.record.clockIn).toLocaleTimeString('en-US', { 
                            hour: '2-digit', 
                            minute: '2-digit',
                            hour12: true 
                          }) : '--:--'} 
                          
                          {" — "}
                          
                          {day.record.clockOut ? new Date(day.record.clockOut).toLocaleTimeString('en-US', { 
                            hour: '2-digit', 
                            minute: '2-digit',
                            hour12: true 
                          }) : 'Present'}
                        </>
                      ) : day.date > today ? (
                        'Upcoming'
                      ) : (
                        'No data'
                      )}
                    </div>
                    {day.hours > 0 && (
                      <span className="font-medium text-gray-700">{day.hours.toFixed(1)}h</span>
                    )}
                  </div>
                  
                  <div className="w-full bg-gray-200 rounded-full h-1.5">
                    {day.hours > 0 && (
                      <div 
                        className={`h-1.5 rounded-full ${
                          day.hours >= 8 ? 'bg-green-500' : (day.hours >= 4 ? 'bg-blue-500' : 'bg-yellow-500')
                        } ${animate ? 'animate-progressGrow' : ''}`}
                        style={{ 
                          width: `${(day.hours / maxHours) * 100}%`,
                          transitionDelay: `${index * 100}ms`
                        }}
                      ></div>
                    )}
                  </div>
                </div>
              </div>
            </div>
          ))}

          <div className="mt-3 pt-2 border-t border-gray-100">
            <div className="flex flex-wrap gap-2 text-xs justify-end">
              <div className="flex items-center">
                <div className="h-2 w-2 bg-green-500 rounded-full mr-1"></div>
                <span>Full day (8h+)</span>
              </div>
              <div className="flex items-center">
                <div className="h-2 w-2 bg-blue-500 rounded-full mr-1"></div>
                <span>Half day (4-8h)</span>
              </div>
              <div className="flex items-center">
                <div className="h-2 w-2 bg-yellow-500 rounded-full mr-1"></div>
                <span>&lt;4h</span>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AttendanceTimings;
