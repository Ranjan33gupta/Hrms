import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { getEmployeeById } from '../services/api';
import { CircularProgress, Paper, Avatar, Box, Typography, Grid, Divider, Button } from '@mui/material';
import { FaUser, FaEnvelope, FaPhone, FaIdCard, FaBuilding, FaBriefcase, FaCalendarAlt } from 'react-icons/fa';

const Profile = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [employeeData, setEmployeeData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEmployeeData = async () => {
      if (!user?.employeeId) {
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        const data = await getEmployeeById(user.employeeId);
        setEmployeeData(data);
      } catch (err) {
        console.error('Error fetching employee data:', err);
        setError('Failed to load profile data. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchEmployeeData();
  }, [user]);

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="50vh">
        <Paper elevation={3} sx={{ p: 4, maxWidth: 500 }}>
          <Typography color="error" variant="h6" gutterBottom>
            {error}
          </Typography>
          <Button variant="contained" onClick={() => navigate(-1)}>
            Go Back
          </Button>
        </Paper>
      </Box>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">My Profile</h1>
        <button
          onClick={() => navigate(-1)}
          className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-md transition-colors"
        >
          Back
        </button>
      </div>

      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="bg-gradient-to-r from-blue-800 to-indigo-900 px-6 py-16 text-white">
          <div className="flex flex-col items-center">
            <div className="w-32 h-32 rounded-full bg-white p-2 shadow-lg mb-4">
              <div className="w-full h-full rounded-full bg-gradient-to-br from-indigo-400 to-purple-500 flex items-center justify-center overflow-hidden">
                {employeeData?.profileImage ? (
                  <img
                    src={employeeData.profileImage}
                    alt={employeeData.fullName || user?.name || 'User'}
                    className="w-full h-full object-cover"
                  />
                ) : (
                  <span className="text-white font-bold text-4xl">
                    {employeeData?.fullName ? employeeData.fullName.charAt(0).toUpperCase() : 
                     user?.name ? user.name.charAt(0).toUpperCase() : 'U'}
                  </span>
                )}
              </div>
            </div>
            <h2 className="text-2xl font-bold">{employeeData?.fullName || user?.name || user?.username || 'User'}</h2>
            <p className="text-indigo-200 mt-1">{employeeData?.designationTitle || user?.role || 'Employee'}</p>
          </div>
        </div>

        <div className="p-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div className="space-y-4">
              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaEnvelope className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Email Address</p>
                  <p className="font-medium">{employeeData?.email || user?.email || 'Not available'}</p>
                </div>
              </div>

              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaPhone className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Phone Number</p>
                  <p className="font-medium">
                    {employeeData?.contactNumber ? 
                      `${employeeData.countryCode || ''} ${employeeData.contactNumber}` : 
                      user?.contactNumber ? 
                        `${user.countryCode || ''} ${user.contactNumber}` : 
                        'Not available'}
                  </p>
                </div>
              </div>

              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaIdCard className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Employee ID</p>
                  <p className="font-medium">{employeeData?.employeeCode || user?.employeeId || 'Not available'}</p>
                </div>
              </div>
            </div>

            <div className="space-y-4">
              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaBuilding className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Department</p>
                  <p className="font-medium">{employeeData?.departmentName || 'Not available'}</p>
                </div>
              </div>

              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaBriefcase className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Position</p>
                  <p className="font-medium">{employeeData?.designationTitle || 'Not available'}</p>
                </div>
              </div>

              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaCalendarAlt className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Joining Date</p>
                  <p className="font-medium">{formatDate(employeeData?.joiningDate)}</p>
                </div>
              </div>
            </div>
          </div>

          {employeeData?.address && (
            <div className="mt-6 pt-6 border-t border-gray-200">
              <div className="flex items-start">
                <div className="bg-indigo-100 p-3 rounded-full mr-4">
                  <FaUser className="text-indigo-600 text-xl" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">Address</p>
                  <p className="font-medium">{employeeData.address}</p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Profile;
