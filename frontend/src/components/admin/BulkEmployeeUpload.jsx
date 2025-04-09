import React, { useState } from 'react';
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
  IconButton,
  Link
} from '@mui/material';
import { 
  CloudUpload as CloudUploadIcon, 
  GetApp as GetAppIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Delete as DeleteIcon
} from '@mui/icons-material';
import { uploadBulkEmployees } from '../../services/api';

const BulkEmployeeUpload = () => {
  const [file, setFile] = useState(null);
  const [previewData, setPreviewData] = useState([]);
  const [uploading, setUploading] = useState(false);
  const [uploadSuccess, setUploadSuccess] = useState(false);
  const [uploadError, setUploadError] = useState(null);
  const [validationErrors, setValidationErrors] = useState([]);

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
          setUploadError(null);
          setValidationErrors([]);
          
          // Validate the data
          const errors = validateEmployeeData(parsedData);
          setValidationErrors(errors);
          
          // Preview the first 5 rows
          setPreviewData(parsedData.slice(0, 5));
        } catch (error) {
          console.error('Error parsing CSV file:', error);
          setUploadError('Failed to parse the CSV file. Please ensure it\'s a valid .csv file.');
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

  const validateEmployeeData = (data) => {
    const errors = [];
    
    // Check if data is empty
    if (!data || data.length === 0) {
      errors.push('The file contains no data');
      return errors;
    }
    
    // Check required fields and data types
    data.forEach((row, index) => {
      const rowNum = index + 2; // +2 because CSV starts at 1 and there's a header row
      
      if (!row.fullName) {
        errors.push(`Row ${rowNum}: Full Name is required`);
      }
      
      if (!row.email) {
        errors.push(`Row ${rowNum}: Email is required`);
      } else if (!isValidEmail(row.email)) {
        errors.push(`Row ${rowNum}: Invalid email format`);
      }
      
      if (!row.department) {
        errors.push(`Row ${rowNum}: Department is required`);
      }
      
      if (!row.designation) {
        errors.push(`Row ${rowNum}: Designation is required`);
      }
      
      // Check date format for dateOfBirth and joiningDate
      if (row.dateOfBirth && !isValidDate(row.dateOfBirth)) {
        errors.push(`Row ${rowNum}: Date of Birth must be in YYYY-MM-DD format`);
      }
      
      if (row.joiningDate && !isValidDate(row.joiningDate)) {
        errors.push(`Row ${rowNum}: Joining Date must be in YYYY-MM-DD format`);
      }
    });
    
    return errors;
  };

  const isValidEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const isValidDate = (dateString) => {
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateRegex.test(dateString)) return false;
    
    const date = new Date(dateString);
    return !isNaN(date.getTime());
  };

  const handleUpload = async () => {
    if (!file || validationErrors.length > 0) return;
    
    try {
      setUploading(true);
      setUploadError(null);
      
      const reader = new FileReader();
      reader.onload = async (e) => {
        const csvData = e.target.result;
        const parsedData = parseCSV(csvData);
        
        try {
          // In a real application, send this data to your API
          await uploadBulkEmployees(parsedData);
          setUploadSuccess(true);
          // Clear file and preview after successful upload
          setFile(null);
          setPreviewData([]);
          document.getElementById('employee-upload-input').value = '';
        } catch (error) {
          console.error('Error uploading employees:', error);
          setUploadError(error.message || 'Failed to upload employees. Please try again.');
        }
      };
      
      reader.readAsText(file);
    } catch (error) {
      console.error('Error reading file:', error);
      setUploadError('Failed to read the file. Please try again.');
    } finally {
      setUploading(false);
    }
  };

  const downloadTemplate = () => {
    // Create a CSV template
    const headers = [
      'fullName',
      'email',
      'phoneNumber',
      'dateOfBirth',
      'gender',
      'address',
      'department',
      'designation',
      'joiningDate',
      'employeeCode',
      'salary',
      'managerId'
    ];
    
    // Sample data row
    const sampleData = [
      'John Doe',
      'john.doe@example.com',
      '+1234567890',
      '1990-01-01',
      'Male',
      '123 Main St, City, Country',
      'Engineering',
      'Software Developer',
      '2023-01-15',
      'EMP001',
      '50000',
      '1'
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
    link.setAttribute('download', 'employee_upload_template.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  const removeFile = () => {
    setFile(null);
    setPreviewData([]);
    setValidationErrors([]);
    setUploadSuccess(false);
    setUploadError(null);
    document.getElementById('employee-upload-input').value = '';
  };

  return (
    <Box>
      <Paper className="p-6 mb-6">
        <Typography variant="h6" className="mb-4">Bulk Employee Upload</Typography>
        
        <Grid container spacing={3} alignItems="center" className="mb-4">
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
                  id="employee-upload-input"
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
              
              {file && (
                <IconButton color="error" onClick={removeFile} className="ml-2">
                  <DeleteIcon />
                </IconButton>
              )}
            </Box>
            
            {file && (
              <Typography variant="body2" className="mt-2">
                Selected file: <strong>{file.name}</strong> ({(file.size / 1024).toFixed(2)} KB)
              </Typography>
            )}
          </Grid>
        </Grid>
        
        {/* Validation errors */}
        {validationErrors.length > 0 && (
          <Alert severity="error" className="mb-4">
            <Typography variant="subtitle2">Please fix the following issues:</Typography>
            <ul className="mt-2 ml-4">
              {validationErrors.slice(0, 5).map((error, index) => (
                <li key={index}>{error}</li>
              ))}
              {validationErrors.length > 5 && (
                <li>...and {validationErrors.length - 5} more errors</li>
              )}
            </ul>
          </Alert>
        )}
        
        {/* Upload success message */}
        {uploadSuccess && (
          <Alert severity="success" className="mb-4" icon={<CheckCircleIcon />}>
            Employees successfully uploaded!
          </Alert>
        )}
        
        {/* Upload error message */}
        {uploadError && (
          <Alert severity="error" className="mb-4" icon={<ErrorIcon />}>
            {uploadError}
          </Alert>
        )}
        
        {/* Data preview */}
        {previewData.length > 0 && (
          <Box className="mt-6">
            <Typography variant="subtitle1" className="mb-2">Preview (first 5 rows):</Typography>
            <TableContainer component={Paper} className="mb-4">
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
                          {row[header]}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
            
            <Box className="flex justify-end">
              <Button
                variant="contained"
                color="primary"
                onClick={handleUpload}
                disabled={uploading || validationErrors.length > 0}
                startIcon={uploading ? <CircularProgress size={20} /> : null}
              >
                {uploading ? 'Uploading...' : 'Upload Employees'}
              </Button>
            </Box>
          </Box>
        )}
        
        {/* Instructions */}
        <Box className="mt-6 p-4 bg-gray-50 rounded-lg">
          <Typography variant="subtitle1" className="mb-2 font-bold">Instructions:</Typography>
          <ol className="list-decimal ml-5 space-y-2">
            <li>Download the CSV template using the button above.</li>
            <li>Fill in the employee details according to the template format.</li>
            <li>Required fields: Full Name, Email, Department, Designation.</li>
            <li>Dates should be in YYYY-MM-DD format (e.g., 1990-01-01).</li>
            <li>Upload the completed CSV file using the "Select CSV File" button.</li>
            <li>Review the data preview and fix any validation errors.</li>
            <li>Click "Upload Employees" to submit the data.</li>
          </ol>
        </Box>
      </Paper>
    </Box>
  );
};

export default BulkEmployeeUpload;
