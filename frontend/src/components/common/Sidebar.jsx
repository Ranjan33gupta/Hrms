import React, { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import {
  FaHome,
  FaUserPlus,
  FaCalendarAlt,
  FaCog,
  FaSignOutAlt,
  FaMoneyBillWave,
  FaUniversity,
  FaCalendarCheck,
  FaClock,
  FaComments
} from 'react-icons/fa';
import axios from 'axios';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  CircularProgress,
  IconButton
} from '@mui/material';
import { Chat, Close } from '@mui/icons-material';
import { apiUrl } from '../../services/apiClient';

const Sidebar = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const [logoHover, setLogoHover] = useState(false);
  const [chatbotOpen, setChatbotOpen] = useState(false);

  // Chatbot states
  const [chatMessages, setChatMessages] = useState([
    { sender: 'bot', text: 'Hello! How can I help you today?' }
  ]);
  const [currentMessage, setCurrentMessage] = useState('');
  const [processingMessage, setProcessingMessage] = useState(false);

  const isActive = (path) => {
    return location.pathname === path;
  };

  const adminLinks = [
    { path: '/dashboard', name: 'Dashboard', icon: <FaHome className="w-5 h-5" /> },
    { path: '/admin-dashboard', name: 'Admin Tools', icon: <FaCog className="w-5 h-5" /> },
    { path: '/add-employee', name: 'Add Employee', icon: <FaUserPlus className="w-5 h-5" /> },
    { path: '/request-leave', name: 'Request Leave', icon: <FaCalendarCheck className="w-5 h-5" /> },
    { path: '/payroll-management', name: 'Payroll', icon: <FaMoneyBillWave className="w-5 h-5" /> },
    { path: '/bank-details-management', name: 'Bank Details', icon: <FaUniversity className="w-5 h-5" /> },
    { path: '/attendance', name: 'Attendance', icon: <FaClock className="w-5 h-5" /> },
    { path: '/calendar', name: 'Calendar', icon: <FaCalendarAlt className="w-5 h-5" /> },
    { path: '/settings', name: 'Settings', icon: <FaCog className="w-5 h-5" /> },
  ];

  const employeeLinks = [
    { path: '/employee-dashboard', name: 'My Dashboard', icon: <FaHome className="w-5 h-5" /> },
    { path: '/attendance', name: 'Attendance', icon: <FaClock className="w-5 h-5" /> },
    { path: '/request-leave', name: 'Request Leave', icon: <FaCalendarCheck className="w-5 h-5" /> },
    { path: '/calendar', name: 'Calendar', icon: <FaCalendarAlt className="w-5 h-5" /> },
    { path: '#', name: 'Chat Assistant', icon: <FaComments className="w-5 h-5" />, action: () => setChatbotOpen(true) },
  ];

  const links = user?.role === 'Admin' ? adminLinks : employeeLinks;

  // Handle sending a message to the chatbot
  const handleSendMessage = async () => {
    if (!currentMessage.trim()) return;

    // Add user message to chat
    const userMessage = { sender: 'user', text: currentMessage };
    setChatMessages(prev => [...prev, userMessage]);

    // Clear input and show processing
    setCurrentMessage('');
    setProcessingMessage(true);

    try {
      // Send message to chatbot API
      const response = await axios.post(`${apiUrl}/Chatbot/Query`, {
        query: userMessage.text,
        employeeId: user?.employeeId
      });

      // Process response
      if (response.data) {
        // Add bot response to chat
        const botMessage = {
          sender: 'bot',
          text: response.data.response || 'I didn\'t understand that. Can you try again?'
        };

        setChatMessages(prev => [...prev, botMessage]);

        // Handle any actions from the bot
        if (response.data.action && response.data.apiEndpoint) {
          try {
            const actionResponse = await axios.get(`${apiUrl}/${response.data.apiEndpoint.replace('/api/', '')}`);
            if (actionResponse.data) {
              const actionMessage = {
                sender: 'bot',
                text: typeof actionResponse.data === 'string'
                  ? actionResponse.data
                  : JSON.stringify(actionResponse.data, null, 2)
              };
              setChatMessages(prev => [...prev, actionMessage]);
            }
          } catch (actionErr) {
            console.error('Error executing chatbot action:', actionErr);
            setChatMessages(prev => [...prev, {
              sender: 'bot',
              text: 'Sorry, I had trouble completing that action.'
            }]);
          }
        }
      }
    } catch (err) {
      console.error('Error sending message to chatbot:', err);
      setChatMessages(prev => [...prev, {
        sender: 'bot',
        text: 'Sorry, I\'m having trouble connecting right now. Please try again later.'
      }]);
    } finally {
      setProcessingMessage(false);
    }
  };

  return (
    <div className="h-screen bg-gray-800 text-white w-64 fixed left-0 top-0 overflow-y-auto">
      <div className="p-5 border-b border-gray-700">
        <div
          className="flex items-center space-x-3 cursor-pointer"
          onMouseEnter={() => setLogoHover(true)}
          onMouseLeave={() => setLogoHover(false)}
        >
          <div className="relative flex items-center justify-center group">
            {/* Background glow effect */}
            <div className={`absolute w-full h-full rounded-full ${logoHover ? 'bg-yellow-300 animate-ping' : 'bg-yellow-400 bg-opacity-20 animate-pulse'}`} style={{ padding: '30%' }}></div>

            {/* Rotating ring effect */}
            <div className={`absolute w-full h-full rounded-full border-2 border-yellow-400 ${logoHover ? 'animate-spin' : ''}`} style={{ padding: '10%' }}></div>

            {/* Logo icon */}
            <svg
              className={`w-12 h-12 text-yellow-400 relative z-10 transition-transform duration-500 ${logoHover ? 'scale-110' : ''}`}
              fill="currentColor"
              viewBox="0 0 20 20"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3zM6 8a2 2 0 11-4 0 2 2 0 014 0zM16 18v-3a5.972 5.972 0 00-.75-2.906A3.005 3.005 0 0119 15v3h-3zM4.75 12.094A5.973 5.973 0 004 15v3H1v-3a3 3 0 013.75-2.906z"></path>
            </svg>
          </div>
          <div className="transition-all duration-300 ease-in-out">
            <h1 className="text-xl font-bold tracking-wider">
              <span className={`text-yellow-400 ${logoHover ? 'animate-bounce' : ''}`}>Work</span>
              <span className="text-white">Nest</span>
              <span className="animate-bounce inline-block ml-1 text-yellow-300">•</span>
            </h1>
            <p className={`text-xs text-gray-400 -mt-1 transition-all duration-300 ${logoHover ? 'translate-x-1' : ''}`}>HR Management System</p>
          </div>
        </div>
      </div>

      <div className="p-5">
        <div className="mb-6">
          <div className="text-gray-400 text-sm mb-2">Welcome,</div>
          <div className="font-semibold">{user?.name || user?.username || 'User'}</div>
          <div className="text-sm text-gray-400">{user?.role || 'User'}</div>
        </div>

        <nav>
          <ul className="space-y-2">
            {links.map((link) => (
              <li key={link.path}>
                {link.action ? (
                  <button
                    onClick={link.action}
                    className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                      isActive(link.path)
                        ? 'bg-blue-700 text-white'
                        : 'text-gray-300 hover:bg-gray-700'
                    }`}
                  >
                    {link.icon}
                    <span>{link.name}</span>
                  </button>
                ) : (
                  <Link
                    to={link.path}
                    className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors ${
                      isActive(link.path)
                        ? 'bg-blue-700 text-white'
                        : 'text-gray-300 hover:bg-gray-700'
                    }`}
                  >
                    {link.icon}
                    <span>{link.name}</span>
                  </Link>
                )}
              </li>
            ))}
          </ul>
        </nav>

        <div className="mt-10 pt-6 border-t border-gray-700">
          <button
            onClick={logout}
            className="flex items-center space-x-3 text-gray-300 hover:text-white w-full px-4 py-3 rounded-lg hover:bg-gray-700 transition-colors"
          >
            <FaSignOutAlt className="w-5 h-5" />
            <span>Logout</span>
          </button>
        </div>
      </div>

      {/* Chatbot Dialog */}
      <Dialog
        open={chatbotOpen}
        onClose={() => setChatbotOpen(false)}
        maxWidth="sm"
        fullWidth
        PaperProps={{
          sx: { height: '70vh' }
        }}
      >
        <DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Box display="flex" alignItems="center">
            <Chat sx={{ mr: 1, color: 'primary.main' }} />
            <Typography variant="h6">WorkNest Assistant</Typography>
          </Box>
          <IconButton onClick={() => setChatbotOpen(false)} size="small">
            <Close />
          </IconButton>
        </DialogTitle>

        <DialogContent sx={{ display: 'flex', flexDirection: 'column', p: 2 }}>
          <Box sx={{ flexGrow: 1, overflowY: 'auto', mb: 2, p: 1 }}>
            {chatMessages.map((msg, index) => (
              <Box
                key={index}
                display="flex"
                justifyContent={msg.sender === 'user' ? 'flex-end' : 'flex-start'}
                mb={1}
              >
                <Paper
                  elevation={1}
                  sx={{
                    p: 1.5,
                    borderRadius: 2,
                    maxWidth: '80%',
                    bgcolor: msg.sender === 'user' ? 'primary.main' : 'grey.100',
                    color: msg.sender === 'user' ? 'white' : 'text.primary'
                  }}
                >
                  <Typography variant="body1">{msg.text}</Typography>
                </Paper>
              </Box>
            ))}
            {processingMessage && (
              <Box display="flex" justifyContent="flex-start" mb={1}>
                <Paper elevation={1} sx={{ p: 1.5, borderRadius: 2, bgcolor: 'grey.100' }}>
                  <Box display="flex" alignItems="center">
                    <CircularProgress size={20} sx={{ mr: 1 }} />
                    <Typography variant="body2">Thinking...</Typography>
                  </Box>
                </Paper>
              </Box>
            )}
          </Box>

          <Box display="flex" alignItems="center">
            <TextField
              fullWidth
              variant="outlined"
              placeholder="Type your message..."
              value={currentMessage}
              onChange={(e) => setCurrentMessage(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
              disabled={processingMessage}
              size="small"
            />
            <Button
              variant="contained"
              color="primary"
              onClick={handleSendMessage}
              disabled={!currentMessage.trim() || processingMessage}
              sx={{ ml: 1 }}
            >
              Send
            </Button>
          </Box>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default Sidebar;
