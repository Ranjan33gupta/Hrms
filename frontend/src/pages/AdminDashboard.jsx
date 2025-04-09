import React, { useState } from 'react';
import { 
  Box, 
  Typography, 
  Grid, 
  Paper, 
  Tabs, 
  Tab, 
  Card, 
  CardContent,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Divider,
  Alert,
  CircularProgress
} from '@mui/material';
import { 
  CloudUpload as CloudUploadIcon, 
  AddCircle as AddCircleIcon,
  Business as BusinessIcon,
  Work as WorkIcon,
  Person as PersonIcon
} from '@mui/icons-material';
import BulkEmployeeUpload from '../components/admin/BulkEmployeeUpload';
import DepartmentManager from '../components/admin/DepartmentManager';
import DesignationManager from '../components/admin/DesignationManager';

const AdminDashboard = () => {
  const [activeTab, setActiveTab] = useState(0);

  const handleTabChange = (event, newValue) => {
    setActiveTab(newValue);
  };

  return (
    <Box className="p-6">
      <Typography variant="h4" gutterBottom component="h1" className="text-gray-800 font-bold mb-6">
        Admin Dashboard
      </Typography>

      <Paper className="mb-6">
        <Tabs
          value={activeTab}
          onChange={handleTabChange}
          indicatorColor="primary"
          textColor="primary"
          centered
        >
          <Tab label="Bulk Employee Upload" icon={<CloudUploadIcon />} iconPosition="start" />
          <Tab label="Department Management" icon={<BusinessIcon />} iconPosition="start" />
          <Tab label="Designation Management" icon={<WorkIcon />} iconPosition="start" />
        </Tabs>
      </Paper>

      {/* Bulk Employee Upload Tab */}
      {activeTab === 0 && (
        <BulkEmployeeUpload />
      )}

      {/* Department Management Tab */}
      {activeTab === 1 && (
        <DepartmentManager />
      )}

      {/* Designation Management Tab */}
      {activeTab === 2 && (
        <DesignationManager />
      )}
    </Box>
  );
};

export default AdminDashboard;
