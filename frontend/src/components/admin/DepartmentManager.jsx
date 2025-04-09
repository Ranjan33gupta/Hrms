import React, { useState, useEffect } from 'react';
import { 
  Box, 
  Typography, 
  Paper, 
  Button, 
  Grid,
  Alert,
  CircularProgress,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  IconButton,
  Divider
} from '@mui/material';
import { 
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  CloudUpload as CloudUploadIcon,
  GetApp as GetAppIcon
} from '@mui/icons-material';
import { 
  getDepartments, 
  createDepartment, 
  updateDepartment, 
  deleteDepartment, 
  uploadBulkDepartments 
} from '../../services/api';

const DepartmentManager = () => {
  const [departments, setDepartments] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [openBulkDialog, setOpenBulkDialog] = useState(false);
  const [currentDepartment, setCurrentDepartment] = useState({ name: '', description: '' });
  const [file, setFile] = useState(null);
  const [previewData, setPreviewData] = useState([]);
  const [uploadSuccess, setUploadSuccess] = useState(false);
  const [validationErrors, setValidationErrors] = useState([]);
  const [isEditMode, setIsEditMode] = useState(false);

  useEffect(() => {
    fetchDepartments();
  }, []);

  const fetchDepartments = async () => {
    try {
      setLoading(true);
      const data = await getDepartments();
      setDepartments(data || []);
    } catch (err) {
      setError('Failed to load departments. Please try again.');
      console.error('Error fetching departments:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCurrentDepartment({ ...currentDepartment, [name]: value });
  };

  const handleAddDepartment = () => {
    setCurrentDepartment({ name: '', description: '' });
    setIsEditMode(false);
    setOpenDialog(true);
  };

  const handleEditDepartment = (department) => {
    setCurrentDepartment(department);
    setIsEditMode(true);
    setOpenDialog(true);
  };

  const handleDeleteDepartment = async (id) => {
    if (!window.confirm('Are you sure you want to delete this department?')) return;
    
    try {
      setLoading(true);
      await deleteDepartment(id);
      setSuccess('Department deleted successfully!');
      fetchDepartments();
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Failed to delete department. Please try again.');
      console.error('Error deleting department:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSaveDepartment = async () => {
    if (!currentDepartment.name.trim()) {
      setError('Department name is required');
      return;
    }
    
    try {
      setLoading(true);
      
      if (isEditMode) {
        await updateDepartment(currentDepartment.id, currentDepartment);
        setSuccess('Department updated successfully!');
      } else {
        await createDepartment(currentDepartment);
        setSuccess('Department created successfully!');
      }
      
      fetchDepartments();
      setOpenDialog(false);
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError(`Failed to ${isEditMode ? 'update' : 'create'} department. Please try again.`);
      console.error(`Error ${isEditMode ? 'updating' : 'creating'} department:`, err);
    } finally {
      setLoading(false);
    }
  };

  const handleBulkUpload = () => {
    setOpenBulkDialog(true);
  };

  const handleFileChange = (event) => {
    const selectedFile = event.target.files[0];
    setFile(selectedFile);
    
    if (selectedFile) {
      const reader = new FileReader();
      reader.onload = (e) => {
        try {
          const csvData = e.target.result;
          const parsedData = parseCSV(csvData);
          
          // Reset previous states
          setUploadSuccess(false);
          setError(null);
          setValidationErrors([]);
          
          // Validate the data
          const errors = validateDepartmentData(parsedData);
          setValidationErrors(errors);
          
          // Preview the first 5 rows
          setPreviewData(parsedData.slice(0, 5));
        } catch (error) {
          console.error('Error parsing CSV file:', error);
          setError('Failed to parse the CSV file. Please ensure it\'s a valid .csv file.');
          setPreviewData([]);
        }
      };
      reader.readAsText(selectedFile);
    } else {
      setPreviewData([]);
    }
  };

  // Parse CSV string into array of objects
  const parseCSV = (csvString) => {
    const lines = csvString.split('\n');
    const headers = lines[0].split(',').map(header => header.trim());
    
    return lines.slice(1).filter(line => line.trim() !== '').map(line => {
      const values = line.split(',').map(value => value.trim());
      const obj = {};
      
      headers.forEach((header, index) => {
        obj[header] = values[index] || '';
      });
      
      return obj;
    });
  };

  const validateDepartmentData = (data) => {
    const errors = [];
    
    // Check if data is empty
    if (!data || data.length === 0) {
      errors.push('The file contains no data');
      return errors;
    }
    
    // Check required fields
    data.forEach((row, index) => {
      const rowNum = index + 2; // +2 because CSV starts at 1 and there's a header row
      
      if (!row.name) {
        errors.push(`Row ${rowNum}: Department name is required`);
      }
    });
    
    return errors;
  };

  const handleUploadDepartments = async () => {
    if (!file || validationErrors.length > 0) return;
    
    try {
      setLoading(true);
      setError(null);
      
      const reader = new FileReader();
      reader.onload = async (e) => {
        const csvData = e.target.result;
        const parsedData = parseCSV(csvData);
        
        try {
          // In a real application, send this data to your API
          await uploadBulkDepartments(parsedData);
          setUploadSuccess(true);
          fetchDepartments();
          // Clear file and preview after successful upload
          setFile(null);
          setPreviewData([]);
          document.getElementById('department-upload-input').value = '';
          
          // Close dialog after a short delay
          setTimeout(() => {
            setOpenBulkDialog(false);
            setUploadSuccess(false);
          }, 2000);
        } catch (error) {
          console.error('Error uploading departments:', error);
          setError(error.message || 'Failed to upload departments. Please try again.');
        }
      };
      
      reader.readAsText(file);
    } catch (error) {
      console.error('Error reading file:', error);
      setError('Failed to read the file. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const downloadTemplate = () => {
    // Create a CSV template
    const headers = [
      'name',
      'description',
      'headOfDepartment',
      'location',
      'budget',
      'established'
    ];
    
    // Sample data row
    const sampleData = [
      'Engineering',
      'Software development and infrastructure',
      'John Doe',
      'Building A, Floor 3',
      '500000',
      '2020-01-01'
    ];
    
    // Create CSV content
    const csvContent = [
      headers.join(','),
      sampleData.join(',')
    ].join('\n');
    
    // Create a blob and download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', 'department_upload_template.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <Box>
      <Paper className="p-6">
        <Box className="flex justify-between items-center mb-4">
          <Typography variant="h6">Department Management</Typography>
          <Box>
            <Button
              variant="outlined"
              startIcon={<CloudUploadIcon />}
              onClick={handleBulkUpload}
              className="mr-2"
            >
              Bulk Upload
            </Button>
            <Button
              variant="contained"
              color="primary"
              startIcon={<AddIcon />}
              onClick={handleAddDepartment}
            >
              Add Department
            </Button>
          </Box>
        </Box>
        
        {error && (
          <Alert severity="error" className="mb-4" onClose={() => setError(null)}>
            {error}
          </Alert>
        )}
        
        {success && (
          <Alert severity="success" className="mb-4" onClose={() => setSuccess(null)}>
            {success}
          </Alert>
        )}
        
        {loading && !departments.length ? (
          <Box className="flex justify-center p-4">
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Department Name</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell>Head of Department</TableCell>
                  <TableCell>Employees</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {departments.length > 0 ? (
                  departments.map((department) => (
                    <TableRow key={department.id}>
                      <TableCell>{department.name}</TableCell>
                      <TableCell>{department.description || 'N/A'}</TableCell>
                      <TableCell>{department.headOfDepartment || 'Not assigned'}</TableCell>
                      <TableCell>{department.employeeCount || 0}</TableCell>
                      <TableCell align="right">
                        <IconButton
                          color="primary"
                          onClick={() => handleEditDepartment(department)}
                        >
                          <EditIcon />
                        </IconButton>
                        <IconButton
                          color="error"
                          onClick={() => handleDeleteDepartment(department.id)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={5} align="center">
                      No departments found
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
      
      {/* Add/Edit Department Dialog */}
      <Dialog 
        open={openDialog} 
        onClose={() => setOpenDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {isEditMode ? 'Edit Department' : 'Add New Department'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2} className="mt-1">
            <Grid item xs={12}>
              <TextField
                name="name"
                label="Department Name *"
                value={currentDepartment.name}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                required
                error={currentDepartment.name.trim() === ''}
                helperText={currentDepartment.name.trim() === '' ? 'Department name is required' : ''}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                name="description"
                label="Description"
                value={currentDepartment.description || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                multiline
                rows={3}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                name="headOfDepartment"
                label="Head of Department"
                value={currentDepartment.headOfDepartment || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                name="location"
                label="Location"
                value={currentDepartment.location || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button 
            onClick={handleSaveDepartment} 
            variant="contained" 
            color="primary"
            disabled={!currentDepartment.name.trim() || loading}
          >
            {loading ? (
              <CircularProgress size={24} />
            ) : isEditMode ? (
              'Update Department'
            ) : (
              'Save Department'
            )}
          </Button>
        </DialogActions>
      </Dialog>
      
      {/* Bulk Upload Dialog */}
      <Dialog
        open={openBulkDialog}
        onClose={() => setOpenBulkDialog(false)}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>Bulk Upload Departments</DialogTitle>
        <DialogContent>
          <Box className="p-4">
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <Box className="flex items-center mb-4">
                  <Button
                    component="label"
                    variant="contained"
                    startIcon={<CloudUploadIcon />}
                    className="mr-4"
                  >
                    Select CSV File
                    <input
                      id="department-upload-input"
                      type="file"
                      accept=".csv"
                      hidden
                      onChange={handleFileChange}
                    />
                  </Button>
                  
                  <Button
                    variant="outlined"
                    startIcon={<GetAppIcon />}
                    onClick={downloadTemplate}
                  >
                    Download Template
                  </Button>
                </Box>
                
                {file && (
                  <Typography variant="body2" className="mt-2">
                    Selected file: <strong>{file.name}</strong> ({(file.size / 1024).toFixed(2)} KB)
                  </Typography>
                )}
              </Grid>
            </Grid>
            
            {error && (
              <Alert severity="error" className="my-4" onClose={() => setError(null)}>
                {error}
              </Alert>
            )}
            
            {uploadSuccess && (
              <Alert severity="success" className="my-4">
                Departments successfully uploaded!
              </Alert>
            )}
            
            {validationErrors.length > 0 && (
              <Alert severity="error" className="my-4">
                <Typography variant="subtitle2">Please fix the following issues:</Typography>
                <ul className="mt-2 ml-4">
                  {validationErrors.map((error, index) => (
                    <li key={index}>{error}</li>
                  ))}
                </ul>
              </Alert>
            )}
            
            {previewData.length > 0 && (
              <Box className="mt-4">
                <Typography variant="subtitle1" className="mb-2">Preview:</Typography>
                <TableContainer component={Paper}>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        {Object.keys(previewData[0]).map((header) => (
                          <TableCell key={header}>{header}</TableCell>
                        ))}
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {previewData.map((row, rowIndex) => (
                        <TableRow key={rowIndex}>
                          {Object.keys(previewData[0]).map((header, cellIndex) => (
                            <TableCell key={`${rowIndex}-${cellIndex}`}>
                              {row[header] || 'N/A'}
                            </TableCell>
                          ))}
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </Box>
            )}
            
            <Divider className="my-4" />
            
            <Box className="mt-4">
              <Typography variant="subtitle2" className="mb-2">Instructions:</Typography>
              <ul className="list-disc ml-4">
                <li>Download the template file using the button above.</li>
                <li>Fill in the department details in the CSV file.</li>
                <li>The 'name' field is required for all departments.</li>
                <li>Upload the completed CSV file.</li>
              </ul>
            </Box>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenBulkDialog(false)}>Cancel</Button>
          <Button
            variant="contained"
            color="primary"
            onClick={handleUploadDepartments}
            disabled={!file || validationErrors.length > 0 || loading}
          >
            {loading ? (
              <CircularProgress size={24} />
            ) : (
              'Upload Departments'
            )}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default DepartmentManager;
