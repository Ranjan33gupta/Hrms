import React, { useState, useRef, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useSidebar } from '../../contexts/SidebarContext';
import { FaInfoCircle, FaUserCircle, FaChevronDown, FaChevronUp, FaBriefcase } from 'react-icons/fa';

const Navbar = () => {
  const { user, logout } = useAuth();
  const { showEmployeeList, showLeaveRequests, resetContent, activeContent } = useSidebar();
  const navigate = useNavigate();
  const location = useLocation();
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [infoDropdownOpen, setInfoDropdownOpen] = useState(false);
  const dropdownRef = useRef(null);
  const infoDropdownRef = useRef(null);

  const handleLogout = () => {
    logout();
    // No need to navigate here, the AuthContext will handle the state change
    // and the ProtectedRoute will redirect automatically
  };

  const toggleDropdown = () => {
    setDropdownOpen(!dropdownOpen);
  };

  const toggleInfoDropdown = () => {
    setInfoDropdownOpen(!infoDropdownOpen);
  };

  const handleEmployeeListClick = () => {
    showEmployeeList();
    setDropdownOpen(false);
  };

  const handleLeaveRequestsClick = () => {
    showLeaveRequests();
    setDropdownOpen(false);
  };

  // Close dropdowns when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
        setDropdownOpen(false);
      }
      if (infoDropdownRef.current && !infoDropdownRef.current.contains(event.target)) {
        setInfoDropdownOpen(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  // Check if user is an admin
  const isAdmin = user?.role === 'Admin';

  return (
    <nav className="bg-gradient-to-r from-indigo-800 via-blue-700 to-purple-800 text-white shadow-xl border-b border-indigo-400 border-opacity-20">
      <div className="container mx-auto px-6 py-3">
        <div className="flex justify-between items-center">
          <div className="flex items-center ml-64">
            {/* Empty space where logo was */}
            <div className="h-9"></div>
          </div>

          <div className="flex items-center space-x-8">
            {/* Info Log Button - Only show for Admin users */}
            {isAdmin && (
              <div className="relative" ref={infoDropdownRef}>
                <button
                  onClick={toggleInfoDropdown}
                  className="flex items-center px-5 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 transition-all duration-300 min-w-[140px] justify-center shadow-md hover:shadow-lg transform hover:-translate-y-0.5"
                  aria-label="Info menu"
                  aria-haspopup="true"
                >
                  <FaInfoCircle className="text-indigo-200 mr-2 text-lg" />
                  <span className="hidden sm:inline font-medium">Info Log</span>
                </button>

                {infoDropdownOpen && (
                  <div className="absolute right-0 mt-2 w-56 rounded-lg shadow-xl bg-white z-50 overflow-hidden border border-indigo-100 transform transition-all duration-200 origin-top-right">
                    <div className="py-2 rounded-md bg-white">
                      <div className="px-4 py-2 bg-indigo-50 text-indigo-800 text-sm font-medium border-b border-indigo-100">Information Center</div>
                      <button
                        onClick={() => {
                          resetContent();
                          setInfoDropdownOpen(false);
                        }}
                        className="block w-full text-left px-4 py-3 text-sm text-gray-700 hover:bg-indigo-50 transition-colors duration-150 flex items-center"
                      >
                        <span className="inline-block w-6 h-6 bg-indigo-100 rounded-full mr-2 flex items-center justify-center text-indigo-600">
                          <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                            <path d="M10.707 2.293a1 1 0 00-1.414 0l-7 7a1 1 0 001.414 1.414L4 10.414V17a1 1 0 001 1h2a1 1 0 001-1v-2a1 1 0 011-1h2a1 1 0 011 1v2a1 1 0 001 1h2a1 1 0 001-1v-6.586l.293.293a1 1 0 001.414-1.414l-7-7z"></path>
                          </svg>
                        </span>
                        Back to Dashboard
                      </button>
                      <button
                        onClick={() => {
                          showEmployeeList();
                          setInfoDropdownOpen(false);
                        }}
                        className="block w-full text-left px-4 py-3 text-sm text-gray-700 hover:bg-indigo-50 transition-colors duration-150 flex items-center"
                      >
                        <span className="inline-block w-6 h-6 bg-indigo-100 rounded-full mr-2 flex items-center justify-center text-indigo-600">
                          <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                            <path d="M9 6a3 3 0 11-6 0 3 3 0 016 0zM17 6a3 3 0 11-6 0 3 3 0 016 0zM12.93 17c.046-.327.07-.66.07-1a6.97 6.97 0 00-1.5-4.33A5 5 0 0119 16v1h-6.07zM6 11a5 5 0 015 5v1H1v-1a5 5 0 015-5z"></path>
                          </svg>
                        </span>
                        Employee List
                      </button>
                      <button
                        onClick={() => {
                          showLeaveRequests();
                          setInfoDropdownOpen(false);
                        }}
                        className="block w-full text-left px-4 py-3 text-sm text-gray-700 hover:bg-indigo-50 transition-colors duration-150 flex items-center"
                      >
                        <span className="inline-block w-6 h-6 bg-indigo-100 rounded-full mr-2 flex items-center justify-center text-indigo-600">
                          <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                            <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd"></path>
                          </svg>
                        </span>
                        Leave Requests
                      </button>
                    </div>
                  </div>
                )}
              </div>
            )}

            {/* User dropdown */}
            <div className="relative" ref={dropdownRef}>
              <button
                onClick={toggleDropdown}
                className="flex items-center space-x-2 focus:outline-none bg-indigo-700 bg-opacity-50 px-4 py-2 rounded-lg hover:bg-opacity-70 transition-all duration-300"
                aria-label="User menu"
                aria-haspopup="true"
              >
                <div className="w-9 h-9 rounded-full bg-gradient-to-br from-indigo-400 to-purple-500 flex items-center justify-center overflow-hidden shadow-inner">
                  {user?.profileImage ? (
                    <img
                      src={user.profileImage}
                      alt={user?.name || 'User'}
                      className="w-full h-full object-cover"
                    />
                  ) : (
                    <span className="text-white font-bold text-sm">
                      {user?.name ? user.name.charAt(0).toUpperCase() : user?.username ? user.username.charAt(0).toUpperCase() : 'U'}
                    </span>
                  )}
                </div>
                <span className="hidden md:inline font-medium">{user?.name || user?.username || 'User'}</span>
                {dropdownOpen ?
                  <FaChevronUp className="text-indigo-200 text-xs" /> :
                  <FaChevronDown className="text-indigo-200 text-xs" />
                }
              </button>

              {dropdownOpen && (
                <div className="absolute right-0 mt-2 w-64 rounded-lg shadow-xl bg-white z-50 overflow-hidden border border-indigo-100 transform transition-all duration-200 origin-top-right">
                  <div className="py-2 rounded-md bg-white">
                    <div className="px-4 py-3 bg-indigo-50 border-b border-indigo-100">
                      <div className="flex items-center">
                        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-400 to-purple-500 flex items-center justify-center overflow-hidden mr-3">
                          {user?.profileImage ? (
                            <img
                              src={user.profileImage}
                              alt={user?.name || 'User'}
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            <span className="text-white font-bold text-sm">
                              {user?.name ? user.name.charAt(0).toUpperCase() : user?.username ? user.username.charAt(0).toUpperCase() : 'U'}
                            </span>
                          )}
                        </div>
                        <div>
                          <div className="text-indigo-800 font-medium">{user?.name || user?.username || 'User'}</div>
                          <div className="text-xs text-indigo-600">{user?.email || 'No email available'}</div>
                        </div>
                      </div>
                    </div>
                    <Link
                      to="/profile"
                      className="block px-4 py-3 text-sm text-gray-700 hover:bg-indigo-50 transition-colors duration-150 flex items-center"
                      onClick={() => setDropdownOpen(false)}
                    >
                      <span className="inline-block w-6 h-6 bg-indigo-100 rounded-full mr-2 flex items-center justify-center text-indigo-600">
                        <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                          <path fillRule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clipRule="evenodd"></path>
                        </svg>
                      </span>
                      Profile
                    </Link>
                    <Link
                      to="/settings"
                      className="block px-4 py-3 text-sm text-gray-700 hover:bg-indigo-50 transition-colors duration-150 flex items-center"
                      onClick={() => setDropdownOpen(false)}
                    >
                      <span className="inline-block w-6 h-6 bg-indigo-100 rounded-full mr-2 flex items-center justify-center text-indigo-600">
                        <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                          <path fillRule="evenodd" d="M11.49 3.17c-.38-1.56-2.6-1.56-2.98 0a1.532 1.532 0 01-2.286.948c-1.372-.836-2.942.734-2.106 2.106.54.886.061 2.042-.947 2.287-1.561.379-1.561 2.6 0 2.978a1.532 1.532 0 01.947 2.287c-.836 1.372.734 2.942 2.106 2.106a1.532 1.532 0 012.287.947c.379 1.561 2.6 1.561 2.978 0a1.533 1.533 0 012.287-.947c1.372.836 2.942-.734 2.106-2.106a1.533 1.533 0 01.947-2.287c1.561-.379 1.561-2.6 0-2.978a1.532 1.532 0 01-.947-2.287c.836-1.372-.734-2.942-2.106-2.106a1.532 1.532 0 01-2.287-.947zM10 13a3 3 0 100-6 3 3 0 000 6z" clipRule="evenodd"></path>
                        </svg>
                      </span>
                      Settings
                    </Link>
                    <button
                      onClick={handleLogout}
                      className="block w-full text-left px-4 py-3 text-sm text-red-600 hover:bg-red-50 transition-colors duration-150 flex items-center border-t border-gray-100 mt-1"
                    >
                      <span className="inline-block w-6 h-6 bg-red-100 rounded-full mr-2 flex items-center justify-center text-red-600">
                        <svg className="w-3 h-3" fill="currentColor" viewBox="0 0 20 20" xmlns="http://www.w3.org/2000/svg">
                          <path fillRule="evenodd" d="M3 3a1 1 0 00-1 1v12a1 1 0 001 1h12a1 1 0 001-1V4a1 1 0 00-1-1H3zm11 3a1 1 0 10-2 0v6.586l-1.293-1.293a1 1 0 10-1.414 1.414l3 3a1 1 0 001.414 0l3-3a1 1 0 00-1.414-1.414L14 12.586V6z" clipRule="evenodd"></path>
                        </svg>
                      </span>
                      Sign out
                    </button>
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
