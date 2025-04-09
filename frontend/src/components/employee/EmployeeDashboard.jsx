import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { getEmployees, getDepartments, getLeaveRequestsByEmployee, getBankDetailByEmployee, clockIn, clockOut } from '../../services/api';
import { getUpcomingHolidays } from '../../services/holidayService';
import AttendanceTracker from '../attendance/AttendanceTracker';
import MoodChangerButton from '../MoodChanger/MoodChangerButton';

const EmployeeDashboard = () => {
  const { user } = useAuth();
  const [employees, setEmployees] = useState([]);
  const [birthdays, setBirthdays] = useState([]);
  const [upcomingBirthdays, setUpcomingBirthdays] = useState([]);
  const [holidays, setHolidays] = useState([]);
  const [onLeaveToday, setOnLeaveToday] = useState([]);
  const [workingRemotely, setWorkingRemotely] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [activeTab, setActiveTab] = useState('organization');
  const [announcements, setAnnouncements] = useState([]);
  const [newJoinees, setNewJoinees] = useState([]);
  const [workAnniversaries, setWorkAnniversaries] = useState([]);
  const [currentTime, setCurrentTime] = useState(new Date());

  useEffect(() => {
    fetchData();
    
    // Update current time every minute
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 60000);
    
    return () => clearInterval(timer);
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Fetch employees
      const employeesData = await getEmployees();
      setEmployees(employeesData);
      
      // Get today's date
      const today = new Date();
      const todayMonth = today.getMonth();
      const todayDate = today.getDate();
      
      // Filter birthdays
      const todayBirthdays = employeesData.filter(emp => {
        if (!emp.dateOfBirth) return false;
        const dob = new Date(emp.dateOfBirth);
        return dob.getMonth() === todayMonth && dob.getDate() === todayDate;
      });
      
      setBirthdays(todayBirthdays);
      
      // Filter upcoming birthdays (next 30 days)
      const upcoming = employeesData.filter(emp => {
        if (!emp.dateOfBirth) return false;
        const dob = new Date(emp.dateOfBirth);
        
        // Create date for this year's birthday
        const thisYearBirthday = new Date(today.getFullYear(), dob.getMonth(), dob.getDate());
        
        // Calculate days difference
        const diffTime = thisYearBirthday - today;
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        
        // Include if birthday is in the next 30 days
        return diffDays > 0 && diffDays <= 30;
      }).sort((a, b) => {
        // Sort by closest birthday
        const dobA = new Date(a.dateOfBirth);
        const dobB = new Date(b.dateOfBirth);
        
        const thisYearBirthdayA = new Date(today.getFullYear(), dobA.getMonth(), dobA.getDate());
        const thisYearBirthdayB = new Date(today.getFullYear(), dobB.getMonth(), dobB.getDate());
        
        return thisYearBirthdayA - thisYearBirthdayB;
      });
      
      setUpcomingBirthdays(upcoming);
      
      // Filter new joinees (joined in the last 30 days)
      const newJoins = employeesData.filter(emp => {
        if (!emp.joiningDate) return false;
        const joinDate = new Date(emp.joiningDate);
        const diffTime = today - joinDate;
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return diffDays >= 0 && diffDays <= 30;
      });
      
      setNewJoinees(newJoins);
      
      // Filter work anniversaries (this month)
      const anniversaries = employeesData.filter(emp => {
        if (!emp.joiningDate) return false;
        const joinDate = new Date(emp.joiningDate);
        return joinDate.getMonth() === todayMonth && joinDate.getDate() === todayDate && joinDate.getFullYear() < today.getFullYear();
      });
      
      setWorkAnniversaries(anniversaries);
      
      // Fetch holidays using the holiday service
      try {
        const holidaysData = await getUpcomingHolidays();
        setHolidays(holidaysData && holidaysData.length > 0 ? holidaysData : []);
      } catch (err) {
        console.error('Error fetching holidays:', err);
        setHolidays([]);
      }
      
      // Fetch other sections
      try {
        const onLeaveData = await getLeaveRequestsByEmployee();
        setOnLeaveToday(onLeaveData);
      } catch (err) {
        console.error('Error fetching on leave:', err);
        setOnLeaveToday([]);
      }
      
      try {
        const workingRemotelyData = await getBankDetailByEmployee();
        setWorkingRemotely(workingRemotelyData);
      } catch (err) {
        console.error('Error fetching working remotely:', err);
        setWorkingRemotely([]);
      }
      
      try {
        const announcementsData = await getDepartments();
        setAnnouncements(announcementsData);
      } catch (err) {
        console.error('Error fetching announcements:', err);
        setAnnouncements([]);
      }
      
    } catch (err) {
      console.error('Error in fetchData:', err);
      setError('Failed to load dashboard data. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (date) => {
    const options = { month: 'short', day: 'numeric' };
    return new Date(date).toLocaleDateString('en-US', options);
  };

  // Render loading state
  if (loading) {
    return (
      <div className="flex justify-center items-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
        <p className="ml-3 text-lg text-gray-700">Loading dashboard...</p>
      </div>
    );
  }

  // Render error state
  if (error) {
    return (
      <div className="flex flex-col items-center justify-center h-screen bg-gray-50">
        <div className="text-red-500 text-5xl mb-4">
          <svg xmlns="http://www.w3.org/2000/svg" className="h-16 w-16" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>
        <h2 className="text-2xl font-bold text-gray-800 mb-2">Dashboard Error</h2>
        <p className="text-gray-600 mb-4">{error}</p>
        <button 
          onClick={fetchData} 
          className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition duration-200"
        >
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div className="bg-gray-100 min-h-screen">
      <div className="container mx-auto px-4 py-8">
        {/* Welcome Banner */}
        <div className="bg-gradient-to-r from-blue-500 to-indigo-600 rounded-lg shadow-lg mb-8 p-6 text-white">
          <div className="flex justify-between items-center">
            <div>
              <h1 className="text-2xl font-bold">Welcome to WorkNest, {user?.fullName || 'Employee'}</h1>
              <p className="mt-1 opacity-90">
                {currentTime.toLocaleDateString('en-US', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
              </p>
            </div>
            <div className="hidden md:block">
              <div className="text-right">
                <p className="text-lg font-semibold">
                  {currentTime.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}
                </p>
                <p className="text-sm opacity-90">
                  {holidays.length > 0 
                    ? `Next holiday: ${holidays[0].name} on ${formatDate(holidays[0].date)}` 
                    : 'No upcoming holidays'}
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
          <div className="bg-white rounded-lg shadow p-4 transition duration-300 hover:shadow-lg">
            <h3 className="text-lg font-semibold mb-2">Attendance</h3>
            <AttendanceTracker />
          </div>
          
          <div className="bg-white rounded-lg shadow p-4 transition duration-300 hover:shadow-lg">
            <h3 className="text-lg font-semibold mb-2">Request Leave</h3>
            <p className="text-gray-600 mb-3">Quick leave application</p>
            <Link to="/request-leave" className="text-blue-500 hover:text-blue-700 flex items-center">
              Apply for leave
              <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </Link>
          </div>
          
          <div className="bg-white rounded-lg shadow p-4 transition duration-300 hover:shadow-lg">
            <h3 className="text-lg font-semibold mb-2">Calendar</h3>
            <p className="text-gray-600 mb-3">View upcoming events</p>
            <Link to="/calendar" className="text-blue-500 hover:text-blue-700 flex items-center">
              Open calendar
              <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 ml-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </Link>
          </div>
          
          <div className="bg-white rounded-lg shadow p-4 transition duration-300 hover:shadow-lg">
            <h3 className="text-lg font-semibold mb-2">How are you feeling?</h3>
            <p className="text-gray-600 mb-3">Share your mood</p>
            <div className="mt-2">
              <MoodChangerButton />
            </div>
          </div>
        </div>

        {/* Main Content Tabs */}
        <div className="bg-white rounded-lg shadow-lg overflow-hidden">
          <div className="flex border-b">
            <button 
              className={`px-4 py-3 text-sm font-medium ${activeTab === 'organization' ? 'border-b-2 border-blue-500 text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
              onClick={() => setActiveTab('organization')}
            >
              Organization
            </button>
            <button 
              className={`px-4 py-3 text-sm font-medium ${activeTab === 'announcements' ? 'border-b-2 border-blue-500 text-blue-600' : 'text-gray-500 hover:text-gray-700'}`}
              onClick={() => setActiveTab('announcements')}
            >
              Announcements
            </button>
          </div>
          
          <div className="p-6">
            {/* Organization Tab */}
            {activeTab === 'organization' && (
              <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2">
                  <div className="bg-gray-50 rounded-lg p-4">
                    <h3 className="text-lg font-semibold mb-4">Organization Overview</h3>
                    
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
                      <div className="bg-white rounded p-3 text-center shadow-sm">
                        <div className="text-2xl font-bold text-blue-600">{employees.length}</div>
                        <div className="text-xs text-gray-500 mt-1">Employees</div>
                      </div>
                      <div className="bg-white rounded p-3 text-center shadow-sm">
                        <div className="text-2xl font-bold text-green-600">{onLeaveToday.length}</div>
                        <div className="text-xs text-gray-500 mt-1">On Leave</div>
                      </div>
                      <div className="bg-white rounded p-3 text-center shadow-sm">
                        <div className="text-2xl font-bold text-purple-600">{workingRemotely.length}</div>
                        <div className="text-xs text-gray-500 mt-1">Remote</div>
                      </div>
                      <div className="bg-white rounded p-3 text-center shadow-sm">
                        <div className="text-2xl font-bold text-orange-600">{birthdays.length}</div>
                        <div className="text-xs text-gray-500 mt-1">Birthdays</div>
                      </div>
                    </div>
                    
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <h4 className="text-sm font-medium mb-3">People on Leave Today</h4>
                        {onLeaveToday.length > 0 ? (
                          <div className="space-y-2">
                            {onLeaveToday.map(employee => (
                              <div key={employee.id} className="flex items-center bg-white p-2 rounded shadow-sm">
                                <img 
                                  src={employee.profileImage || 'https://via.placeholder.com/40'} 
                                  alt={employee.fullName} 
                                  className="w-8 h-8 rounded-full mr-3"
                                />
                                <div>
                                  <div className="text-sm font-medium">{employee.fullName}</div>
                                  <div className="text-xs text-gray-500">{employee.reason}</div>
                                </div>
                              </div>
                            ))}
                          </div>
                        ) : (
                          <div className="bg-white p-4 rounded text-center text-gray-500">
                            No one is on leave today
                          </div>
                        )}
                      </div>
                      
                      <div>
                        <h4 className="text-sm font-medium mb-3">People Working Remotely</h4>
                        {workingRemotely.length > 0 ? (
                          <div className="space-y-2">
                            {workingRemotely.map(employee => (
                              <div key={employee.id} className="flex items-center bg-white p-2 rounded shadow-sm">
                                <img 
                                  src={employee.profileImage || 'https://via.placeholder.com/40'}
                                  alt={employee.fullName} 
                                  className="w-8 h-8 rounded-full mr-3"
                                />
                                <div className="text-sm font-medium">{employee.fullName}</div>
                              </div>
                            ))}
                          </div>
                        ) : (
                          <div className="bg-white p-4 rounded text-center text-gray-500">
                            No one is working remotely today
                          </div>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
                
                <div>
                  <div className="bg-gray-50 rounded-lg p-4 h-full">
                    <div className="flex justify-between items-center mb-4">
                      <h3 className="text-lg font-semibold">Events & Celebrations</h3>
                      <div className="flex space-x-2">
                        <div className="flex items-center text-sm">
                          <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-pink-500 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 15.546c-.523 0-1.046.151-1.5.454a2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.701 2.701 0 00-1.5-.454M9 6v2m3-2v2m3-2v2M9 3h.01M12 3h.01M15 3h.01M21 21v-7a2 2 0 00-2-2H5a2 2 0 00-2 2v7h18zm-3-9v-2a2 2 0 00-2-2H8a2 2 0 00-2 2v2h12z" />
                          </svg>
                          <span className="text-sm font-medium">{birthdays.length} Birthdays</span>
                        </div>
                        <div className="flex items-center text-sm">
                          <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-blue-500 mr-1" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 13.255A23.931 23.931 0 0112 15c-3.183 0-6.22-.62-9-1.745M16 6V4a2 2 0 00-2-2h-4a2 2 0 00-2 2v2m4 6h.01M5 20h14a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                          </svg>
                          <span className="text-sm font-medium">{workAnniversaries.length} Work Anniversaries</span>
                        </div>
                      </div>
                    </div>
                    
                    <div className="mt-4">
                      <h3 className="text-sm font-medium mb-2">Birthdays today</h3>
                      {birthdays.length > 0 ? (
                        <div className="space-y-2">
                          {birthdays.map(employee => (
                            <div key={employee.id} className="bg-gray-50 p-3 rounded-lg flex items-center">
                              <img 
                                src={employee.profileImage || 'https://via.placeholder.com/40'} 
                                alt={employee.fullName} 
                                className="w-8 h-8 rounded-full mr-3"
                              />
                              <span className="text-sm">{employee.fullName}</span>
                            </div>
                          ))}
                        </div>
                      ) : (
                        <div className="text-center py-6">
                          <img 
                            src="https://cdn-icons-png.flaticon.com/512/1458/1458279.png" 
                            alt="No birthdays" 
                            className="mx-auto h-16 w-auto opacity-50"
                          />
                          <p className="text-gray-500 text-sm mt-2">No birthdays today</p>
                        </div>
                      )}
                    </div>
                    
                    <div className="mt-6">
                      <h3 className="text-sm font-medium mb-2">Upcoming Birthdays</h3>
                      <div className="grid grid-cols-5 gap-2">
                        {upcomingBirthdays.slice(0, 5).map(employee => (
                          <div key={employee.id} className="text-center">
                            <div className="relative">
                              <img 
                                src={employee.profileImage || 'https://via.placeholder.com/50'} 
                                alt={employee.fullName} 
                                className="w-12 h-12 rounded-full mx-auto"
                              />
                              <div className="absolute bottom-0 right-0 bg-blue-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                {new Date(employee.dateOfBirth).getDate()}
                              </div>
                            </div>
                            <p className="text-xs mt-1 truncate">{employee.fullName.split(' ')[0]}</p>
                            <p className="text-xs text-gray-500 truncate">
                              {new Date(employee.dateOfBirth).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                            </p>
                          </div>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            )}
            
            {/* Announcements Tab */}
            {activeTab === 'announcements' && (
              <div className="space-y-6">
                {announcements.length > 0 ? (
                  announcements.map(announcement => (
                    <div key={announcement.id} className="border-l-4 border-blue-500 bg-blue-50 p-4 rounded">
                      <div className="flex justify-between items-start">
                        <div>
                          <h3 className="text-lg font-medium text-blue-800">{announcement.title}</h3>
                          <p className="mt-1 text-sm text-gray-600">{announcement.description}</p>
                        </div>
                        <div className="text-xs text-gray-500">
                          {new Date(announcement.date).toLocaleDateString()}
                        </div>
                      </div>
                    </div>
                  ))
                ) : (
                  <div className="text-center py-8">
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-12 w-12 mx-auto text-gray-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z" />
                    </svg>
                    <p className="text-gray-500">No announcements available at this time</p>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default EmployeeDashboard;
