import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import { 
  Card, CardContent, Typography, Grid, Button, Box, 
  Paper, Avatar, Divider, CircularProgress, IconButton,
  Dialog, DialogTitle, DialogContent, DialogActions,
  TextField, MenuItem, Select, FormControl, InputLabel
} from '@mui/material';
import { 
  AccessTime, LocationOn, Mood, MoodBad, SentimentSatisfied,
  SentimentVeryDissatisfied, SentimentVerySatisfied, Camera, 
  Chat, FormatQuote, Close
} from '@mui/icons-material';

// Import service file for API calls
import { apiUrl } from '../../services/apiClient';

const EmployeeDashboard = ({ employeeId, employeeName }) => {
  // States for different features
  const [loading, setLoading] = useState(true);
  const [todayAttendance, setTodayAttendance] = useState(null);
  const [clockInTime, setClockInTime] = useState(null);
  const [clockOutTime, setClockOutTime] = useState(null);
  const [location, setLocation] = useState(null);
  const [error, setError] = useState(null);
  
  // Camera states
  const [showCamera, setShowCamera] = useState(false);
  const [isCameraReady, setIsCameraReady] = useState(false);
  const [isClockIn, setIsClockIn] = useState(true);
  const videoRef = useRef(null);
  const canvasRef = useRef(null);
  const [cameraError, setCameraError] = useState(null);
  
  // Chatbot states
  const [showChatbot, setShowChatbot] = useState(false);
  const [chatMessages, setChatMessages] = useState([
    { sender: 'bot', text: 'Hello! How can I help you today?' }
  ]);
  const [currentMessage, setCurrentMessage] = useState('');
  const [processingMessage, setProcessingMessage] = useState(false);
  
  // Mood tracking states
  const [showMoodDialog, setShowMoodDialog] = useState(false);
  const [currentMood, setCurrentMood] = useState(2); // Default to Neutral
  const [moodComment, setMoodComment] = useState('');
  const [moodHistory, setMoodHistory] = useState([]);
  
  // Motivational quote state
  const [quote, setQuote] = useState(null);
  const [showQuote, setShowQuote] = useState(false);

  // Fetch today's attendance when component mounts
  useEffect(() => {
    fetchTodayAttendance();
    fetchRandomQuote();
    fetchMoodHistory();
  }, [employeeId]);

  // Show quote after clock-in
  useEffect(() => {
    if (clockInTime && !clockOutTime && !showQuote && quote) {
      // Show quote 2 seconds after clock-in
      const timer = setTimeout(() => {
        setShowQuote(true);
      }, 2000);
      
      return () => clearTimeout(timer);
    }
  }, [clockInTime, clockOutTime, quote]);

  const fetchTodayAttendance = async () => {
    try {
      setLoading(true);
      const response = await axios.get(`${apiUrl}/Attendance/Employee/${employeeId}/Today`);
      
      if (response.data) {
        setTodayAttendance(response.data);
        setClockInTime(response.data.clockIn);
        setClockOutTime(response.data.clockOut);
      }
    } catch (err) {
      if (err.response && err.response.status === 404) {
        // No attendance record for today, which is normal
        setTodayAttendance(null);
      } else {
        setError('Error fetching attendance data');
        console.error('Error fetching attendance:', err);
      }
      // Continue execution even when API call fails
      setTodayAttendance(null);
    } finally {
      setLoading(false);
    }
  };

  const fetchRandomQuote = async () => {
    try {
      const response = await axios.get(`${apiUrl}/MotivationalQuote/Random`);
      if (response.data) {
        setQuote(response.data);
      }
    } catch (err) {
      console.error('Error fetching quote:', err);
      // Set a default quote if API call fails
      setQuote({ 
        text: "The only way to do great work is to love what you do.", 
        author: "Steve Jobs" 
      });
    }
  };

  const fetchMoodHistory = async () => {
    try {
      const response = await axios.get(`${apiUrl}/Mood/Employee/${employeeId}`);
      if (response.data) {
        setMoodHistory(response.data);
      }
    } catch (err) {
      console.error('Error fetching mood history:', err);
      // Set empty mood history if API call fails
      setMoodHistory([]);
    }
  };

  const handleClockIn = async () => {
    // First show camera for photo capture
    setIsClockIn(true);
    startCamera();
  };

  const handleClockOut = async () => {
    setIsClockIn(false);
    startCamera();
  };

  const startCamera = async () => {
    setShowCamera(true);
    setCameraError(null);
    
    try {
      const constraints = {
        video: {
          width: { ideal: 1280 },
          height: { ideal: 720 },
          facingMode: 'user'
        }
      };
      
      const stream = await navigator.mediaDevices.getUserMedia(constraints);
      
      if (videoRef.current) {
        videoRef.current.srcObject = stream;
        setIsCameraReady(true);
      }
    } catch (err) {
      setCameraError('Could not access camera. Please check permissions.');
      console.error('Camera error:', err);
    }
  };

  const stopCamera = () => {
    if (videoRef.current && videoRef.current.srcObject) {
      const tracks = videoRef.current.srcObject.getTracks();
      tracks.forEach(track => track.stop());
      videoRef.current.srcObject = null;
    }
    setShowCamera(false);
    setIsCameraReady(false);
  };

  const capturePhoto = () => {
    if (!isCameraReady || !videoRef.current || !canvasRef.current) {
      return;
    }
    
    const video = videoRef.current;
    const canvas = canvasRef.current;
    const context = canvas.getContext('2d');
    
    // Set canvas dimensions to match video
    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;
    
    // Draw the video frame to the canvas
    context.drawImage(video, 0, 0, canvas.width, canvas.height);
    
    // Convert canvas to blob
    canvas.toBlob(async (blob) => {
      if (!blob) {
        setCameraError('Failed to capture photo');
        return;
      }
      
      // Get current location
      let locationData = null;
      try {
        const position = await getCurrentPosition();
        locationData = {
          latitude: position.coords.latitude,
          longitude: position.coords.longitude
        };
      } catch (err) {
        console.error('Location error:', err);
      }
      
      // Stop the camera
      stopCamera();
      
      // Now proceed with clock in/out
      if (isClockIn) {
        processClockIn(blob, locationData);
      } else {
        processClockOut(blob, locationData);
      }
    }, 'image/jpeg', 0.8);
  };

  const getCurrentPosition = () => {
    return new Promise((resolve, reject) => {
      if (!navigator.geolocation) {
        reject(new Error('Geolocation is not supported by your browser'));
      } else {
        navigator.geolocation.getCurrentPosition(resolve, reject, {
          enableHighAccuracy: true,
          timeout: 5000,
          maximumAge: 0
        });
      }
    });
  };

  const processClockIn = async (photoBlob, locationData) => {
    try {
      setLoading(true);
      
      // First clock in
      const clockInData = {
        employeeId: employeeId,
        checkInLatitude: locationData?.latitude,
        checkInLongitude: locationData?.longitude,
        checkInLocation: locationData ? `${locationData.latitude.toFixed(6)}, ${locationData.longitude.toFixed(6)}` : null,
        checkInDevice: navigator.userAgent,
        checkInIpAddress: 'client-side' // Actual IP will be determined server-side
      };
      
      const clockInResponse = await axios.post(`${apiUrl}/Attendance/ClockIn`, clockInData);
      
      if (clockInResponse.data) {
        // Now upload the photo
        const formData = new FormData();
        formData.append('AttendanceId', clockInResponse.data.id);
        formData.append('IsClockIn', 'true');
        formData.append('Photo', photoBlob, 'clock-in.jpg');
        formData.append('DeviceInfo', navigator.userAgent);
        
        await axios.post(`${apiUrl}/AttendancePhoto/Upload`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });
        
        setClockInTime(clockInResponse.data.clockIn);
        setTodayAttendance(clockInResponse.data);
        
        // Ensure we have a motivational quote
        if (!quote) {
          await fetchRandomQuote();
        }
        
        // Show mood dialog after clock in
        setTimeout(() => {
          setShowMoodDialog(true);
        }, 1000);
        
        // Quote will be shown automatically by the useEffect
      }
    } catch (err) {
      setError('Error clocking in: ' + (err.response?.data || err.message));
      console.error('Clock in error:', err);
    } finally {
      setLoading(false);
    }
  };

  const processClockOut = async (photoBlob, locationData) => {
    try {
      setLoading(true);
      
      // First clock out
      const clockOutData = {
        employeeId: employeeId,
        checkOutLatitude: locationData?.latitude,
        checkOutLongitude: locationData?.longitude,
        checkOutLocation: locationData ? `${locationData.latitude.toFixed(6)}, ${locationData.longitude.toFixed(6)}` : null,
        checkOutDevice: navigator.userAgent,
        checkOutIpAddress: 'client-side' // Actual IP will be determined server-side
      };
      
      const clockOutResponse = await axios.put(`${apiUrl}/Attendance/ClockOut`, clockOutData);
      
      if (clockOutResponse.data) {
        // Now upload the photo
        const formData = new FormData();
        formData.append('AttendanceId', clockOutResponse.data.id);
        formData.append('IsClockIn', 'false');
        formData.append('Photo', photoBlob, 'clock-out.jpg');
        formData.append('DeviceInfo', navigator.userAgent);
        
        await axios.post(`${apiUrl}/AttendancePhoto/Upload`, formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        });
        
        setClockOutTime(clockOutResponse.data.clockOut);
        setTodayAttendance(clockOutResponse.data);
        
        // Show mood dialog after clock out
        setTimeout(() => {
          setShowMoodDialog(true);
        }, 1000);
      }
    } catch (err) {
      setError('Error clocking out: ' + (err.response?.data || err.message));
      console.error('Clock out error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleChatbotToggle = () => {
    setShowChatbot(!showChatbot);
  };

  const handleSendMessage = async () => {
    if (!currentMessage.trim() || processingMessage) return;
    
    // Add user message to chat
    const userMessage = { sender: 'user', text: currentMessage };
    setChatMessages(prev => [...prev, userMessage]);
    setCurrentMessage('');
    setProcessingMessage(true);
    
    try {
      // Send message to chatbot API
      const response = await axios.post(`${apiUrl}/Chatbot/Query`, {
        message: userMessage.text,
        employeeId: employeeId
      });
      
      // Add bot response to chat
      const botMessage = { 
        sender: 'bot', 
        text: response.data.message,
        intent: response.data.intent,
        requiresAction: response.data.requiresAction,
        apiEndpoint: response.data.apiEndpoint
      };
      
      setChatMessages(prev => [...prev, botMessage]);
      
      // If the bot response requires an action (e.g., fetching leave balance)
      if (botMessage.requiresAction && botMessage.apiEndpoint) {
        // Handle the action based on the API endpoint
        // This is a simplified example
        try {
          const actionResponse = await axios.get(`${apiUrl}/${botMessage.apiEndpoint.replace('/api/', '')}`);
          
          // Add the action response to the chat
          setChatMessages(prev => [
            ...prev, 
            { 
              sender: 'bot', 
              text: `Here's what I found: ${JSON.stringify(actionResponse.data)}`,
              isActionResult: true
            }
          ]);
        } catch (actionErr) {
          setChatMessages(prev => [
            ...prev, 
            { 
              sender: 'bot', 
              text: 'Sorry, I couldn\'t complete that action right now.',
              isError: true
            }
          ]);
        }
      }
    } catch (err) {
      // Add error message to chat
      setChatMessages(prev => [
        ...prev, 
        { 
          sender: 'bot', 
          text: 'Sorry, I\'m having trouble understanding right now. Please try again later.',
          isError: true
        }
      ]);
      console.error('Chatbot error:', err);
    } finally {
      setProcessingMessage(false);
    }
  };

  const handleMoodSubmit = async () => {
    try {
      await axios.post(`${apiUrl}/Mood`, {
        employeeId: employeeId,
        mood: currentMood,
        comment: moodComment
      });
      
      setShowMoodDialog(false);
      setMoodComment('');
      
      // Refresh mood history
      fetchMoodHistory();
    } catch (err) {
      console.error('Error submitting mood:', err);
    }
  };

  const getMoodIcon = (mood) => {
    switch (mood) {
      case 0: return <SentimentVeryDissatisfied color="error" fontSize="large" />;
      case 1: return <MoodBad color="warning" fontSize="large" />;
      case 2: return <SentimentSatisfied color="info" fontSize="large" />;
      case 3: return <Mood color="success" fontSize="large" />;
      case 4: return <SentimentVerySatisfied color="success" fontSize="large" />;
      default: return <SentimentSatisfied color="info" fontSize="large" />;
    }
  };

  const getMoodText = (mood) => {
    switch (mood) {
      case 0: return 'Very Negative';
      case 1: return 'Negative';
      case 2: return 'Neutral';
      case 3: return 'Positive';
      case 4: return 'Very Positive';
      default: return 'Neutral';
    }
  };

  return (
    <div className="employee-dashboard">
      {error && (
        <Paper elevation={3} sx={{ p: 2, mb: 2, bgcolor: '#ffebee' }}>
          <Typography color="error">{error}</Typography>
        </Paper>
      )}
      
      <Grid container spacing={3}>
        {/* Attendance Card */}
        <Grid item xs={12} md={6}>
          <Card elevation={4}>
            <CardContent>
              <Typography variant="h5" gutterBottom>
                Attendance
              </Typography>
              
              {loading ? (
                <Box display="flex" justifyContent="center" my={3}>
                  <CircularProgress />
                </Box>
              ) : (
                <>
                  <Box display="flex" alignItems="center" mb={2}>
                    <AccessTime color="primary" sx={{ mr: 1 }} />
                    <Typography variant="body1">
                      {new Date().toLocaleDateString('en-US', { 
                        weekday: 'long', 
                        year: 'numeric', 
                        month: 'long', 
                        day: 'numeric' 
                      })}
                    </Typography>
                  </Box>
                  
                  <Box sx={{ mb: 2 }}>
                    <Typography variant="body1">
                      <strong>Clock In:</strong> {clockInTime ? new Date(`2000-01-01T${clockInTime}`).toLocaleTimeString() : 'Not clocked in'}
                    </Typography>
                    
                    <Typography variant="body1">
                      <strong>Clock Out:</strong> {clockOutTime ? new Date(`2000-01-01T${clockOutTime}`).toLocaleTimeString() : 'Not clocked out'}
                    </Typography>
                    
                    {todayAttendance?.hoursWorked !== undefined && (
                      <Typography variant="body1">
                        <strong>Hours Worked:</strong> {todayAttendance.hoursWorked.toFixed(2)}
                      </Typography>
                    )}
                  </Box>
                  
                  <Box display="flex" justifyContent="space-between" mt={3}>
                    <Button 
                      variant="contained" 
                      color="primary"
                      disabled={!!clockInTime || loading}
                      onClick={handleClockIn}
                      startIcon={<AccessTime />}
                    >
                      Clock In
                    </Button>
                    
                    <Button 
                      variant="contained" 
                      color="secondary"
                      disabled={!clockInTime || !!clockOutTime || loading}
                      onClick={handleClockOut}
                      startIcon={<AccessTime />}
                    >
                      Clock Out
                    </Button>
                  </Box>
                </>
              )}
            </CardContent>
          </Card>
        </Grid>
        
        {/* Mood Tracking Card */}
        <Grid item xs={12} md={6}>
          <Card elevation={4}>
            <CardContent>
              <Typography variant="h5" gutterBottom>
                Mood Tracker
              </Typography>
              
              <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="body1">How are you feeling today?</Typography>
                <Button 
                  variant="outlined" 
                  color="primary"
                  onClick={() => setShowMoodDialog(true)}
                  startIcon={<Mood />}
                >
                  Record Mood
                </Button>
              </Box>
              
              {moodHistory.length > 0 ? (
                <Box>
                  <Typography variant="subtitle1" gutterBottom>Recent Mood History:</Typography>
                  <Box sx={{ maxHeight: 200, overflowY: 'auto' }}>
                    {moodHistory.slice(0, 5).map((entry, index) => (
                      <Box key={index} display="flex" alignItems="center" mb={1}>
                        {getMoodIcon(entry.mood)}
                        <Box ml={1}>
                          <Typography variant="body2">
                            {new Date(entry.entryDate).toLocaleDateString()} - {getMoodText(entry.mood)}
                          </Typography>
                          {entry.comment && (
                            <Typography variant="caption" color="textSecondary">
                              "{entry.comment}"
                            </Typography>
                          )}
                        </Box>
                      </Box>
                    ))}
                  </Box>
                </Box>
              ) : (
                <Typography variant="body2" color="textSecondary">
                  No mood entries yet. Start tracking your mood!
                </Typography>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
      
      {/* Camera Dialog */}
      <Dialog 
        open={showCamera} 
        onClose={stopCamera}
        maxWidth="md"
        fullWidth
      >
        <DialogTitle>
          {isClockIn ? 'Clock In' : 'Clock Out'} Photo
          <IconButton
            aria-label="close"
            onClick={stopCamera}
            sx={{ position: 'absolute', right: 8, top: 8 }}
          >
            <Close />
          </IconButton>
        </DialogTitle>
        
        <DialogContent>
          {cameraError ? (
            <Typography color="error">{cameraError}</Typography>
          ) : (
            <Box display="flex" flexDirection="column" alignItems="center">
              <video
                ref={videoRef}
                autoPlay
                playsInline
                style={{ width: '100%', maxHeight: '60vh', borderRadius: 8 }}
              />
              <canvas ref={canvasRef} style={{ display: 'none' }} />
              
              <Button
                variant="contained"
                color="primary"
                onClick={capturePhoto}
                disabled={!isCameraReady}
                startIcon={<Camera />}
                sx={{ mt: 2 }}
              >
                Capture Photo
              </Button>
            </Box>
          )}
        </DialogContent>
      </Dialog>
      
      {/* Chatbot Dialog */}
      <Dialog
        open={showChatbot}
        onClose={handleChatbotToggle}
        maxWidth="sm"
        fullWidth
        PaperProps={{
          sx: { height: '70vh', maxHeight: 600 }
        }}
      >
        <DialogTitle>
          HRMS Assistant
          <IconButton
            aria-label="close"
            onClick={handleChatbotToggle}
            sx={{ position: 'absolute', right: 8, top: 8 }}
          >
            <Close />
          </IconButton>
        </DialogTitle>
        
        <DialogContent dividers sx={{ display: 'flex', flexDirection: 'column', p: 2, flexGrow: 1 }}>
          <Box sx={{ flexGrow: 1, overflowY: 'auto', mb: 2 }}>
            {chatMessages.map((msg, index) => (
              <Box
                key={index}
                sx={{
                  display: 'flex',
                  justifyContent: msg.sender === 'user' ? 'flex-end' : 'flex-start',
                  mb: 1
                }}
              >
                <Paper
                  elevation={1}
                  sx={{
                    p: 1.5,
                    maxWidth: '70%',
                    borderRadius: 2,
                    bgcolor: msg.sender === 'user' ? 'primary.light' : 'grey.100',
                    color: msg.sender === 'user' ? 'white' : 'text.primary',
                    ...(msg.isError && { bgcolor: '#ffebee' }),
                    ...(msg.isActionResult && { bgcolor: '#e3f2fd' })
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
      
      {/* Mood Dialog */}
      <Dialog
        open={showMoodDialog}
        onClose={() => setShowMoodDialog(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>
          How are you feeling today?
        </DialogTitle>
        
        <DialogContent>
          <Box display="flex" justifyContent="space-between" mb={3} mt={1}>
            {[0, 1, 2, 3, 4].map((mood) => (
              <IconButton
                key={mood}
                onClick={() => setCurrentMood(mood)}
                sx={{
                  p: 1,
                  border: currentMood === mood ? '2px solid' : 'none',
                  borderColor: 'primary.main',
                  borderRadius: 2
                }}
              >
                {getMoodIcon(mood)}
              </IconButton>
            ))}
          </Box>
          
          <Typography variant="body2" gutterBottom>
            Selected: <strong>{getMoodText(currentMood)}</strong>
          </Typography>
          
          <TextField
            fullWidth
            multiline
            rows={3}
            variant="outlined"
            label="Comments (optional)"
            placeholder="Share how you're feeling..."
            value={moodComment}
            onChange={(e) => setMoodComment(e.target.value)}
            margin="normal"
          />
        </DialogContent>
        
        <DialogActions>
          <Button onClick={() => setShowMoodDialog(false)}>Cancel</Button>
          <Button onClick={handleMoodSubmit} variant="contained" color="primary">
            Submit
          </Button>
        </DialogActions>
      </Dialog>
      
      {/* Motivational Quote Dialog */}
      <Dialog
        open={showQuote}
        onClose={() => setShowQuote(false)}
        maxWidth="sm"
        PaperProps={{
          sx: { bgcolor: '#f5f5f5', borderRadius: 3 }
        }}
      >
        <DialogContent>
          <Box textAlign="center" py={2}>
            <FormatQuote sx={{ fontSize: 40, color: 'primary.main', transform: 'rotate(180deg)' }} />
            <Typography variant="h6" gutterBottom sx={{ fontStyle: 'italic', mb: 2 }}>
              {quote?.quoteText}
            </Typography>
            {quote?.author && (
              <Typography variant="subtitle1" color="textSecondary">
                — {quote.author}
              </Typography>
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setShowQuote(false)}>Close</Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default EmployeeDashboard;
