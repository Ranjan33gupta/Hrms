import React, { useState, useEffect } from 'react';
import { getUpcomingHolidays, formatHolidaysForCalendar } from '../services/holidayService';

// We'll implement a simple calendar since we couldn't install react-big-calendar
const Calendar = () => {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentMonth, setCurrentMonth] = useState(new Date());
  
  // Get month and year names
  const monthNames = ["January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"];
  
  useEffect(() => {
    const fetchHolidays = async () => {
      try {
        setLoading(true);
        const holidays = await getUpcomingHolidays();
        const formattedEvents = formatHolidaysForCalendar(holidays);
        setEvents(formattedEvents);
        setError(null);
      } catch (err) {
        setError('Failed to load holidays. Please try again later.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    
    fetchHolidays();
  }, []);
  
  // Navigate to previous month
  const prevMonth = () => {
    setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() - 1, 1));
  };
  
  // Navigate to next month
  const nextMonth = () => {
    setCurrentMonth(new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1, 1));
  };
  
  // Get days in month
  const getDaysInMonth = (year, month) => {
    return new Date(year, month + 1, 0).getDate();
  };
  
  // Get day of week for first day of month (0 = Sunday, 6 = Saturday)
  const getFirstDayOfMonth = (year, month) => {
    return new Date(year, month, 1).getDay();
  };
  
  // Check if a date has events
  const getEventsForDate = (date) => {
    return events.filter(event => {
      const eventDate = new Date(event.start);
      return eventDate.getDate() === date && 
             eventDate.getMonth() === currentMonth.getMonth() && 
             eventDate.getFullYear() === currentMonth.getFullYear();
    });
  };
  
  // Render calendar
  const renderCalendar = () => {
    const year = currentMonth.getFullYear();
    const month = currentMonth.getMonth();
    const daysInMonth = getDaysInMonth(year, month);
    const firstDayOfMonth = getFirstDayOfMonth(year, month);
    
    // Create blank cells for days before first day of month
    const blanks = [];
    for (let i = 0; i < firstDayOfMonth; i++) {
      blanks.push(
        <div key={`blank-${i}`} className="bg-gray-100 p-2 h-24 border"></div>
      );
    }
    
    // Create cells for each day in month
    const days = [];
    for (let d = 1; d <= daysInMonth; d++) {
      const date = new Date(year, month, d);
      const today = new Date();
      const isToday = date.getDate() === today.getDate() && 
                      date.getMonth() === today.getMonth() && 
                      date.getFullYear() === today.getFullYear();
      
      const dayEvents = getEventsForDate(d);
      
      days.push(
        <div key={d} className={`p-2 border h-24 overflow-y-auto ${isToday ? 'bg-blue-100' : ''}`}>
          <div className="font-bold">{d}</div>
          {dayEvents.map(event => (
            <div 
              key={event.id} 
              className={`p-1 mb-1 text-xs rounded truncate ${
                event.type === 0 ? 'bg-green-200' : 
                event.type === 1 ? 'bg-red-200' : 'bg-yellow-200'
              }`}
              title={`${event.title}${event.description ? ': ' + event.description : ''}`}
            >
              {event.title}
            </div>
          ))}
        </div>
      );
    }
    
    // Combine blanks and days
    const totalSlots = [...blanks, ...days];
    const rows = [];
    let cells = [];
    
    // Create rows with 7 cells each (for each day of week)
    totalSlots.forEach((cell, i) => {
      if (i % 7 !== 0) {
        cells.push(cell);
      } else {
        rows.push(cells);
        cells = [];
        cells.push(cell);
      }
      if (i === totalSlots.length - 1) {
        rows.push(cells);
      }
    });
    
    return rows.map((row, i) => (
      <div key={i} className="grid grid-cols-7">
        {row}
      </div>
    ));
  };
  
  return (
    <div className="container mx-auto p-4">
      <h1 className="text-2xl font-bold mb-4">Company Calendar</h1>
      
      <div className="mb-4 flex justify-between items-center">
        <button 
          onClick={prevMonth}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
        >
          Previous
        </button>
        <h2 className="text-xl font-semibold">
          {monthNames[currentMonth.getMonth()]} {currentMonth.getFullYear()}
        </h2>
        <button 
          onClick={nextMonth}
          className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
        >
          Next
        </button>
      </div>
      
      {loading ? (
        <div className="text-center py-4">Loading calendar...</div>
      ) : error ? (
        <div className="text-center py-4 text-red-500">{error}</div>
      ) : (
        <div className="border rounded">
          {/* Calendar header - days of week */}
          <div className="grid grid-cols-7 bg-gray-200 font-bold">
            <div className="p-2 border text-center">Sun</div>
            <div className="p-2 border text-center">Mon</div>
            <div className="p-2 border text-center">Tue</div>
            <div className="p-2 border text-center">Wed</div>
            <div className="p-2 border text-center">Thu</div>
            <div className="p-2 border text-center">Fri</div>
            <div className="p-2 border text-center">Sat</div>
          </div>
          
          {/* Calendar body */}
          {renderCalendar()}
        </div>
      )}
      
      {/* Legend */}
      <div className="mt-4 flex gap-4">
        <div className="flex items-center">
          <div className="w-4 h-4 bg-green-200 mr-2"></div>
          <span>Company Holiday</span>
        </div>
        <div className="flex items-center">
          <div className="w-4 h-4 bg-red-200 mr-2"></div>
          <span>Government Holiday</span>
        </div>
        <div className="flex items-center">
          <div className="w-4 h-4 bg-yellow-200 mr-2"></div>
          <span>Optional Holiday</span>
        </div>
      </div>
      
      {/* Upcoming Holidays */}
      <div className="mt-8">
        <h3 className="text-xl font-semibold mb-2">Upcoming Holidays</h3>
        <div className="bg-white shadow overflow-hidden rounded-md">
          <ul className="divide-y divide-gray-200">
            {events.length === 0 ? (
              <li className="px-6 py-4">No upcoming holidays</li>
            ) : (
              events.slice(0, 5).map(event => (
                <li key={event.id} className="px-6 py-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <p className="font-medium">{event.title}</p>
                      <p className="text-sm text-gray-500">
                        {event.description || 'No description available'}
                      </p>
                    </div>
                    <div>
                      <span className={`px-2 py-1 text-xs rounded-full ${
                        event.type === 0 ? 'bg-green-200' : 
                        event.type === 1 ? 'bg-red-200' : 'bg-yellow-200'
                      }`}>
                        {event.type === 0 ? 'Company' : 
                         event.type === 1 ? 'Government' : 'Optional'}
                      </span>
                      <p className="text-sm text-gray-500 mt-1">
                        {new Date(event.start).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                </li>
              ))
            )}
          </ul>
        </div>
      </div>
    </div>
  );
};

export default Calendar;
