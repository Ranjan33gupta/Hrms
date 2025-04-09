import { Routes, Route, Navigate } from 'react-router-dom';
import { useState, useEffect } from 'react'
import Dashboard from './pages/Dashboard';
import AdminDashboard from './pages/AdminDashboard';
import AddEmployee from './pages/AddEmployee';
import RequestLeave from './pages/RequestLeave';
import Login from './pages/Login';
import Signup from './pages/Signup';
import Settings from './pages/Settings';
import Profile from './pages/Profile';
import EmployeePage from './pages/EmployeePage';
import Calendar from './pages/Calendar';
import PayrollManagement from './pages/PayrollManagement';
import BankDetailsManagement from './pages/BankDetailsManagement';
import AttendanceDashboard from './pages/AttendanceDashboard';
import EmployeeHistory from './components/employee/EmployeeHistory';
import PayrollHistory from './components/employee/PayrollHistory';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { SidebarProvider } from './contexts/SidebarContext';
import { ChatbotProvider } from './contexts/ChatbotContext';
import Navbar from './components/common/Navbar';
import Sidebar from './components/common/Sidebar';
import ChatbotWidget from './components/Chatbot/ChatbotWidget';
import ProtectedRoute from './components/ProtectedRoute';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import ErrorBoundary from './components/ui/ErrorBoundary';
import { ToastProvider } from './components/ui/Toast';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { CircularProgress, Box } from '@mui/material';

// Import i18n
import './i18n/i18n';
import { useTranslation } from 'react-i18next';

// Create theme
const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#dc004e',
    },
    background: {
      default: '#f5f5f5',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: 'none',
        },
      },
    },
  },
});

function App() {
  return (
    <ErrorBoundary>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <ToastProvider>
          <AuthProvider>
            <SidebarProvider>
              <ChatbotProvider>
                <AppContent />
                <ToastContainer position="top-right" autoClose={3000} />
                <ChatbotWidget />
              </ChatbotProvider>
            </SidebarProvider>
          </AuthProvider>
        </ToastProvider>
      </ThemeProvider>
    </ErrorBoundary>
  );
}

function AppContent() {
  const { user, isAuthenticated, loading } = useAuth();
  const { t } = useTranslation();

  // Don't render anything while authentication is being checked
  if (loading) {
    return (
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh'
        }}
      >
        <CircularProgress />
        <Box sx={{ ml: 2 }}>{t('common.loading')}</Box>
      </Box>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {isAuthenticated && <Navbar />}
      {isAuthenticated && <Sidebar />}

      <main className={`${isAuthenticated ? 'ml-64' : ''} container mx-auto px-4 py-8`}>
        <ErrorBoundary>
          <Routes>
            <Route path="/login" element={
              isAuthenticated ?
                (user?.role === 'Admin' ?
                  <Navigate to="/dashboard" replace /> :
                  <Navigate to="/employee-dashboard" replace />
                ) :
                <Login />
            } />

            <Route path="/signup" element={
              isAuthenticated ?
                (user?.role === 'Admin' ?
                  <Navigate to="/dashboard" replace /> :
                  <Navigate to="/employee-dashboard" replace />
                ) :
                <Signup />
            } />

            {/* Protected Routes */}
            <Route path="/dashboard" element={
              <ProtectedRoute requiredRole="Admin">
                <Dashboard />
              </ProtectedRoute>
            } />

            <Route path="/admin-dashboard" element={
              <ProtectedRoute requiredRole="Admin">
                <AdminDashboard />
              </ProtectedRoute>
            } />

            <Route path="/add-employee" element={
              <ProtectedRoute requiredRole="Admin">
                <AddEmployee />
              </ProtectedRoute>
            } />

            <Route path="/settings" element={
              <ProtectedRoute requiredRole="Admin">
                <Settings />
              </ProtectedRoute>
            } />

            <Route path="/payroll-management" element={
              <ProtectedRoute requiredRole="Admin">
                <PayrollManagement />
              </ProtectedRoute>
            } />

            <Route path="/bank-details-management" element={
              <ProtectedRoute requiredRole="Admin">
                <BankDetailsManagement />
              </ProtectedRoute>
            } />

            <Route path="/employee-dashboard" element={
              <ProtectedRoute>
                <EmployeePage />
              </ProtectedRoute>
            } />

            <Route path="/request-leave" element={
              <ProtectedRoute>
                <RequestLeave />
              </ProtectedRoute>
            } />

            <Route path="/calendar" element={
              <ProtectedRoute>
                <Calendar />
              </ProtectedRoute>
            } />

            <Route path="/attendance" element={
              <ProtectedRoute>
                <AttendanceDashboard />
              </ProtectedRoute>
            } />

            <Route path="/profile" element={
              <ProtectedRoute>
                <Profile />
              </ProtectedRoute>
            } />

            {/* Employee View Route */}
            <Route path="/employees/:id" element={
              <ProtectedRoute>
                <EmployeePage />
              </ProtectedRoute>
            } />

            {/* Employee Edit Route */}
            <Route path="/employees/edit/:id" element={
              <ProtectedRoute requiredRole="Admin">
                <AddEmployee />
              </ProtectedRoute>
            } />

            {/* Employee History Route */}
            <Route path="/employees/:id/history" element={
              <ProtectedRoute>
                <EmployeeHistory />
              </ProtectedRoute>
            } />

            {/* Payroll History Route */}
            <Route path="/employees/:id/payroll-history" element={
              <ProtectedRoute>
                <PayrollHistory />
              </ProtectedRoute>
            } />

            {/* Default Route */}
            <Route path="/" element={
              isAuthenticated ?
                (user?.role === 'Admin' ?
                  <Navigate to="/dashboard" replace /> :
                  <Navigate to="/employee-dashboard" replace />
                ) :
                <Navigate to="/login" replace />
            } />

            {/* Catch all other routes */}
            <Route path="*" element={
              <Navigate to="/" replace />
            } />
          </Routes>
        </ErrorBoundary>
      </main>

      <footer className="bg-gray-100 py-4 mt-8">
        <div className="container mx-auto px-4 text-center text-gray-600">
          <p>&copy; {new Date().getFullYear()} WorkNest. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
}

export default App
