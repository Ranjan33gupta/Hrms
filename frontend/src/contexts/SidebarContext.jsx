import React, { createContext, useContext, useState } from 'react';

const SidebarContext = createContext();

export const SidebarProvider = ({ children }) => {
  // activeContent can be 'dashboard', 'employees', or 'leaveRequests'
  const [activeContent, setActiveContent] = useState('dashboard');

  const showEmployeeList = () => {
    setActiveContent('employees');
  };

  const showLeaveRequests = () => {
    setActiveContent('leaveRequests');
  };

  const resetContent = () => {
    setActiveContent('dashboard');
  };

  return (
    <SidebarContext.Provider value={{ 
      activeContent, 
      showEmployeeList, 
      showLeaveRequests, 
      resetContent 
    }}>
      {children}
    </SidebarContext.Provider>
  );
};

export const useSidebar = () => useContext(SidebarContext);
