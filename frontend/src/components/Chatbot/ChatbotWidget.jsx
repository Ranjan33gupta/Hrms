import React, { useRef, useEffect } from 'react';
import {
  Box,
  Paper,
  Typography,
  TextField,
  Button,
  IconButton,
  Fab,
  Drawer,
  CircularProgress,
  Tooltip,
  Zoom,
  Avatar,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip
} from '@mui/material';
import {
  Chat as ChatIcon,
  Close as CloseIcon,
  Send as SendIcon,
  Mic as MicIcon,
  MicOff as MicOffIcon,
  NavigateNext as NavigateNextIcon,
  Help as HelpIcon,
  Settings as SettingsIcon,
  History as HistoryIcon
} from '@mui/icons-material';
import { useChatbot } from '../../contexts/ChatbotContext';
import { styled, keyframes } from '@mui/system';

// Animations
const fadeIn = keyframes`
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
`;

const pulseAnimation = keyframes`
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
`;

// Styled components
const AnimatedMessage = styled(Paper)(({ theme, isuser, iserror, islocal }) => ({
  padding: theme.spacing(1.5),
  borderRadius: isuser === 'true' ? '18px 18px 4px 18px' : '18px 18px 18px 4px',
  maxWidth: '80%',
  marginBottom: theme.spacing(1),
  backgroundColor: isuser === 'true'
    ? theme.palette.primary.main
    : iserror === 'true'
      ? theme.palette.error.light
      : islocal === 'true'
        ? theme.palette.info.light
        : theme.palette.grey[100],
  color: isuser === 'true'
    ? theme.palette.primary.contrastText
    : iserror === 'true'
      ? theme.palette.error.contrastText
      : theme.palette.text.primary,
  animation: `${fadeIn} 0.3s ease-out`,
  boxShadow: iserror === 'true'
    ? '0 2px 8px rgba(211,47,47,0.15)'
    : islocal === 'true'
      ? '0 2px 8px rgba(3,169,244,0.15)'
      : theme.shadows[1],
  wordBreak: 'break-word',
  border: iserror === 'true'
    ? `1px solid ${theme.palette.error.main}`
    : islocal === 'true'
      ? `1px solid ${theme.palette.info.main}`
      : 'none'
}));

const ChatbotFab = styled(Fab)(({ theme }) => ({
  position: 'fixed',
  bottom: theme.spacing(3),
  right: theme.spacing(3),
  zIndex: 1000,
  boxShadow: theme.shadows[4],
  backgroundColor: theme.palette.primary.main,
  '&:hover': {
    backgroundColor: theme.palette.primary.dark,
    animation: `${pulseAnimation} 1s infinite`
  }
}));

const ChatHeader = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  backgroundColor: theme.palette.primary.main,
  color: theme.palette.primary.contrastText,
  borderRadius: '8px 8px 0 0',
  display: 'flex',
  justifyContent: 'space-between',
  alignItems: 'center'
}));

const MessageList = styled(Box)(({ theme }) => ({
  flexGrow: 1,
  overflowY: 'auto',
  padding: theme.spacing(2),
  display: 'flex',
  flexDirection: 'column'
}));

const InputArea = styled(Box)(({ theme }) => ({
  padding: theme.spacing(2),
  borderTop: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
  display: 'flex',
  alignItems: 'center'
}));

const TypingIndicator = styled(Box)(({ theme }) => ({
  display: 'flex',
  alignItems: 'center',
  padding: theme.spacing(1),
  borderRadius: 18,
  backgroundColor: theme.palette.grey[100],
  width: 'fit-content',
  marginBottom: theme.spacing(1)
}));

const Dot = styled('span')(({ theme, delay }) => ({
  width: 8,
  height: 8,
  backgroundColor: theme.palette.primary.main,
  borderRadius: '50%',
  margin: '0 2px',
  animation: `${pulseAnimation} 1s infinite ${delay}s`
}));

// Main component
const ChatbotWidget = () => {
  const {
    isOpen,
    messages,
    inputText,
    isProcessing,
    isRecording,
    toggleChatbot,
    closeChatbot,
    handleInputChange,
    handleSendMessage,
    handleVoiceInput,
    getSuggestions
  } = useChatbot();

  const messageListRef = useRef(null);
  const [showSuggestions, setShowSuggestions] = React.useState(true);

  // Scroll to bottom when messages change
  useEffect(() => {
    if (messageListRef.current) {
      messageListRef.current.scrollTop = messageListRef.current.scrollHeight;
    }
  }, [messages]);

  // Show suggestions when opening the chatbot
  useEffect(() => {
    if (isOpen) {
      setShowSuggestions(true);
    }
  }, [isOpen]);

  const handleKeyPress = (e) => {
    if (e.key === 'Enter' && !e.shiftKey && inputText.trim()) {
      e.preventDefault();
      handleSendMessage();
      setShowSuggestions(false);
    }
  };

  const handleSuggestionClick = (suggestion) => {
    handleSendMessage(suggestion);
    setShowSuggestions(false);
  };

  const handleRetry = (failedMessage) => {
    // Remove the error message
    setMessages(prev => prev.filter(msg =>
      !(msg.isError && msg.id.toString().includes('error'))
    ));

    // Resend the failed message
    handleSendMessage(failedMessage);
  };

  return (
    <>
      {/* Chatbot toggle button */}
      <ChatbotFab
        color="primary"
        aria-label="chat"
        onClick={toggleChatbot}
      >
        <ChatIcon />
      </ChatbotFab>

      {/* Chatbot drawer */}
      <Drawer
        anchor="right"
        open={isOpen}
        onClose={closeChatbot}
        PaperProps={{
          sx: {
            width: { xs: '100%', sm: 400 },
            height: '100%',
            borderRadius: 0
          }
        }}
      >
        <Box display="flex" flexDirection="column" height="100%">
          {/* Header */}
          <ChatHeader>
            <Box display="flex" alignItems="center">
              <Avatar sx={{ bgcolor: 'primary.dark', mr: 1 }}>
                <ChatIcon />
              </Avatar>
              <Box>
                <Typography variant="h6">WorkNest Assistant</Typography>
                <Typography variant="caption">AI-powered chatbot</Typography>
              </Box>
            </Box>
            <IconButton
              color="inherit"
              onClick={closeChatbot}
              size="small"
            >
              <CloseIcon />
            </IconButton>
          </ChatHeader>

          {/* Messages */}
          <MessageList ref={messageListRef}>
            {messages.map((msg) => (
              <Box
                key={msg.id}
                alignSelf={msg.sender === 'user' ? 'flex-end' : 'flex-start'}
                width="100%"
              >
                <AnimatedMessage
                  isuser={msg.sender === 'user' ? 'true' : 'false'}
                  iserror={msg.isError ? 'true' : 'false'}
                  islocal={msg.isLocalResponse ? 'true' : 'false'}
                >
                  <Typography variant="body1">{msg.text}</Typography>
                  {msg.isApiResult && (
                    <Typography variant="caption" color="text.secondary">
                      Data retrieved from API
                    </Typography>
                  )}
                  {msg.isLocalResponse && (
                    <Box display="flex" alignItems="center" mt={1}>
                      <Typography variant="caption" color="info.main">
                        <Box component="span" sx={{ display: 'flex', alignItems: 'center' }}>
                          <Box component="span" sx={{ mr: 0.5, display: 'flex' }}>
                            <svg width="12" height="12" viewBox="0 0 24 24">
                              <path fill="currentColor" d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 15h-2v-6h2v6zm0-8h-2V7h2v2z"/>
                            </svg>
                          </Box>
                          Offline mode - using local responses
                        </Box>
                      </Typography>
                    </Box>
                  )}
                  {msg.isError && (
                    <Box sx={{ mt: 1 }}>
                      <Box display="flex" alignItems="center">
                        <Typography variant="caption" color="error">
                          <Box component="span" sx={{ display: 'flex', alignItems: 'center' }}>
                            <Box component="span" sx={{ mr: 0.5, display: 'flex' }}>
                              <svg width="12" height="12" viewBox="0 0 24 24">
                                <path fill="currentColor" d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm1 15h-2v-2h2v2zm0-4h-2V7h2v6z"/>
                              </svg>
                            </Box>
                            Connection issue
                          </Box>
                        </Typography>
                      </Box>
                      <Button
                        size="small"
                        variant="outlined"
                        color="error"
                        sx={{ mt: 1, fontSize: '0.75rem', py: 0.5 }}
                        onClick={() => {
                          // Find the last user message before this error
                          const userMessages = messages.filter(m => m.sender === 'user');
                          if (userMessages.length > 0) {
                            const lastUserMessage = userMessages[userMessages.length - 1];
                            handleRetry(lastUserMessage.text);
                          }
                        }}
                      >
                        Try Again
                      </Button>
                    </Box>
                  )}
                </AnimatedMessage>
              </Box>
            ))}

            {isProcessing && (
              <Box alignSelf="flex-start">
                <TypingIndicator>
                  <Dot delay={0} />
                  <Dot delay={0.2} />
                  <Dot delay={0.4} />
                </TypingIndicator>
              </Box>
            )}

            {/* Quick suggestions */}
            {showSuggestions && !isProcessing && messages.length < 3 && (
              <Box mt={2}>
                <Typography variant="caption" color="text.secondary" sx={{ mb: 1 }}>
                  Try asking:
                </Typography>
                <Box display="flex" flexWrap="wrap" gap={1}>
                  {getSuggestions().map((suggestion, index) => (
                    <Chip
                      key={index}
                      label={suggestion}
                      onClick={() => handleSuggestionClick(suggestion)}
                      clickable
                      color="primary"
                      variant="outlined"
                      size="small"
                    />
                  ))}
                </Box>
              </Box>
            )}
          </MessageList>

          {/* Input area */}
          <InputArea>
            <TextField
              fullWidth
              placeholder="Type your message..."
              variant="outlined"
              size="small"
              value={inputText}
              onChange={handleInputChange}
              onKeyPress={handleKeyPress}
              disabled={isProcessing || isRecording}
              InputProps={{
                endAdornment: (
                  <Tooltip title={isRecording ? "Stop recording" : "Voice input"}>
                    <IconButton
                      color={isRecording ? "secondary" : "default"}
                      onClick={handleVoiceInput}
                      edge="end"
                    >
                      {isRecording ? <MicOffIcon /> : <MicIcon />}
                    </IconButton>
                  </Tooltip>
                )
              }}
            />
            <Button
              variant="contained"
              color="primary"
              endIcon={<SendIcon />}
              onClick={() => {
                handleSendMessage();
                setShowSuggestions(false);
              }}
              disabled={!inputText.trim() || isProcessing || isRecording}
              sx={{ ml: 1, minWidth: 100 }}
            >
              Send
            </Button>
          </InputArea>
        </Box>
      </Drawer>
    </>
  );
};

export default ChatbotWidget;
