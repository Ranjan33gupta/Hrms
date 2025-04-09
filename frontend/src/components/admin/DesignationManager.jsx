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
  getDesignations, 
  createDesignation, 
  updateDesignation, 
  deleteDesignation, 
  uploadBulkDesignations 
} from '../../services/api';

const DesignationManager = () => {
  const [designations, setDesignations] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [openDialog, setOpenDialog] = useState(false);
  const [openBulkDialog, setOpenBulkDialog] = useState(false);
  const [currentDesignation, setCurrentDesignation] = useState({ title: '', description: '', department: '', level: '' });
  const [file, setFile] = useState(null);
  const [previewData, setPreviewData] = useState([]);
  const [uploadSuccess, setUploadSuccess] = useState(false);
  const [validationErrors, setValidationErrors] = useState([]);
  const [isEditMode, setIsEditMode] = useState(false);

  useEffect(() => {
    fetchDesignations();
  }, []);

  const fetchDesignations = async () => {
    try {
      setLoading(true);
      const data = await getDesignations();
      setDesignations(data || []);
    } catch (err) {
      setError('Failed to load designations. Please try again.');
      console.error('Error fetching designations:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setCurrentDesignation({ ...currentDesignation, [name]: value });
  };

  const handleAddDesignation = () => {
    setCurrentDesignation({ title: '', description: '', department: '', level: '' });
    setIsEditMode(false);
    setOpenDialog(true);
  };

  const handleEditDesignation = (designation) => {
    setCurrentDesignation(designation);
    setIsEditMode(true);
    setOpenDialog(true);
  };

  const handleDeleteDesignation = async (id) => {
    if (!window.confirm('Are you sure you want to delete this designation?')) return;
    
    try {
      setLoading(true);
      await deleteDesignation(id);
      setSuccess('Designation deleted successfully!');
      fetchDesignations();
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Failed to delete designation. Please try again.');
      console.error('Error deleting designation:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSaveDesignation = async () => {
    if (!currentDesignation.title.trim()) {
      setError('Designation title is required');
      return;
    }
    
    try {
      setLoading(true);
      
      if (isEditMode) {
        await updateDesignation(currentDesignation.id, currentDesignation);
        setSuccess('Designation updated successfully!');
      } else {
        await createDesignation(currentDesignation);
        setSuccess('Designation created successfully!');
      }
      
      fetchDesignations();
      setOpenDialog(false);
      
      // Clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError(`Failed to ${isEditMode ? 'update' : 'create'} designation. Please try again.`);
      console.error(`Error ${isEditMode ? 'updating' : 'creating'} designation:`, err);
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
          const errors = validateDesignationData(parsedData);
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

  const validateDesignationData = (data) => {
    const errors = [];
    
    // Check if data is empty
    if (!data || data.length === 0) {
      errors.push('The file contains no data');
      return errors;
    }
    
    // Check required fields
    data.forEach((row, index) => {
      const rowNum = index + 2; // +2 because CSV starts at 1 and there's a header row
      
      if (!row.title) {
        errors.push(`Row ${rowNum}: Designation title is required`);
      }
      
      if (!row.department) {
        errors.push(`Row ${rowNum}: Department is required`);
      }
    });
    
    return errors;
  };

  const handleUploadDesignations = async () => {
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
          await uploadBulkDesignations(parsedData);
          setUploadSuccess(true);
          fetchDesignations();
          // Clear file and preview after successful upload
          setFile(null);
          setPreviewData([]);
          document.getElementById('designation-upload-input').value = '';
          
          // Close dialog after a short delay
          setTimeout(() => {
            setOpenBulkDialog(false);
            setUploadSuccess(false);
          }, 2000);
        } catch (error) {
          console.error('Error uploading designations:', error);
          setError(error.message || 'Failed to upload designations. Please try again.');
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
      'title',
      'description',
      'department',
      'level',
      'minSalary',
      'maxSalary'
    ];
    
    // Sample data row
    const sampleData = [
      'Software Developer',
      'Develops and maintains software applications',
      'Engineering',
      'Mid-level',
      '60000',
      '90000'
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
    link.setAttribute('download', 'designation_upload_template.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  return (
    <Box>
      <Paper className="p-6">
        <Box className="flex justify-between items-center mb-4">
          <Typography variant="h6">Designation Management</Typography>
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
              onClick={handleAddDesignation}
            >
              Add Designation
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
        
        {loading && !designations.length ? (
          <Box className="flex justify-center p-4">
            <CircularProgress />
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Title</TableCell>
                  <TableCell>Department</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell>Level</TableCell>
                  <TableCell>Salary Range</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {designations.length > 0 ? (
                  designations.map((designation) => (
                    <TableRow key={designation.id}>
                      <TableCell>{designation.title}</TableCell>
                      <TableCell>{designation.department || 'N/A'}</TableCell>
                      <TableCell>{designation.description || 'N/A'}</TableCell>
                      <TableCell>{designation.level || 'N/A'}</TableCell>
                      <TableCell>
                        {designation.minSalary && designation.maxSalary
                          ? `$${designation.minSalary} - $${designation.maxSalary}`
                          : 'Not specified'}
                      </TableCell>
                      <TableCell align="right">
                        <IconButton
                          color="primary"
                          onClick={() => handleEditDesignation(designation)}
                        >
                          <EditIcon />
                        </IconButton>
                        <IconButton
                          color="error"
                          onClick={() => handleDeleteDesignation(designation.id)}
                        >
                          <DeleteIcon />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      No designations found
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
      
      {/* Add/Edit Designation Dialog */}
      <Dialog 
        open={openDialog} 
        onClose={() => setOpenDialog(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          {isEditMode ? 'Edit Designation' : 'Add New Designation'}
        </DialogTitle>
        <DialogContent>
          <Grid container spacing={2} className="mt-1">
            <Grid item xs={12}>
              <TextField
                name="title"
                label="Designation Title *"
                value={currentDesignation.title}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                required
                error={currentDesignation.title.trim() === ''}
                helperText={currentDesignation.title.trim() === '' ? 'Designation title is required' : ''}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                name="department"
                label="Department *"
                value={currentDesignation.department || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                required
                error={currentDesignation.department?.trim() === ''}
                helperText={currentDesignation.department?.trim() === '' ? 'Department is required' : ''}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                name="description"
                label="Description"
                value={currentDesignation.description || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                multiline
                rows={3}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                name="level"
                label="Level"
                value={currentDesignation.level || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                placeholder="e.g. Entry, Mid, Senior"
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                name="minSalary"
                label="Minimum Salary"
                value={currentDesignation.minSalary || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                type="number"
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                name="maxSalary"
                label="Maximum Salary"
                value={currentDesignation.maxSalary || ''}
                onChange={handleInputChange}
                fullWidth
                variant="outlined"
                type="number"
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button 
            onClick={handleSaveDesignation} 
            variant="contained" 
            color="primary"
            disabled={!currentDesignation.title.trim() || !currentDesignation.department?.trim() || loading}
          >
            {loading ? (
              <CircularProgress size={24} />
            ) : isEditMode ? (
              'Update Designation'
            ) : (
              'Save Designation'
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
        <DialogTitle>Bulk Upload Designations</DialogTitle>
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
                      id="designation-upload-input"
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
                Designations successfully uploaded!
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
                <li>Fill in the designation details in the CSV file.</li>
                <li>The 'title' and 'department' fields are required for all designations.</li>
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
            onClick={handleUploadDesignations}
            disabled={!file || validationErrors.length > 0 || loading}
          >
            {loading ? (
              <CircularProgress size={24} />
            ) : (
              'Upload Designations'
            )}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default DesignationManager;
