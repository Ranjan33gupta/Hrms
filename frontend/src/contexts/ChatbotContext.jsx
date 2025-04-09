import React, { createContext, useState, useContext, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from './AuthContext';
import apiClient from '../services/apiClient';
import fallbackChatbotService from '../services/fallbackChatbotService';

const ChatbotContext = createContext();

export const useChatbot = () => useContext(ChatbotContext);

export const ChatbotProvider = ({ children }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [messages, setMessages] = useState([
    { id: 'welcome', sender: 'bot', text: 'Hello! I\'m your WorkNest assistant. How can I help you today?' }
  ]);
  const [inputText, setInputText] = useState('');
  const [isProcessing, setIsProcessing] = useState(false);
  const [conversationId, setConversationId] = useState(null);
  const [isRecording, setIsRecording] = useState(false);
  const [recognition, setRecognition] = useState(null);

  const { user } = useAuth();
  const navigate = useNavigate();

  // Initialize speech recognition
  useEffect(() => {
    if ('webkitSpeechRecognition' in window) {
      const recognitionInstance = new window.webkitSpeechRecognition();
      recognitionInstance.continuous = false;
      recognitionInstance.interimResults = false;
      recognitionInstance.lang = 'en-US';

      recognitionInstance.onresult = (event) => {
        const transcript = event.results[0][0].transcript;
        setInputText(transcript);
        setIsRecording(false);
        // Auto-send voice message
        handleSendMessage(transcript);
      };

      recognitionInstance.onerror = (event) => {
        console.error('Speech recognition error', event.error);
        setIsRecording(false);
      };

      recognitionInstance.onend = () => {
        setIsRecording(false);
      };

      setRecognition(recognitionInstance);
    }
  }, []);

  const openChatbot = () => setIsOpen(true);
  const closeChatbot = () => setIsOpen(false);
  const toggleChatbot = () => setIsOpen(prev => !prev);

  const handleInputChange = (e) => {
    setInputText(e.target.value);
  };

  const handleSendMessage = async (voiceText) => {
    const messageText = voiceText || inputText;
    if (!messageText.trim()) return;

    // Add user message to chat
    const userMessage = { id: Date.now().toString(), sender: 'user', text: messageText };
    setMessages(prev => [...prev, userMessage]);
    setInputText('');
    setIsProcessing(true);

    // Set a timeout to show an error message if the request takes too long
    const timeoutId = setTimeout(() => {
      if (isProcessing) {
        setIsProcessing(false);
        setMessages(prev => [...prev, {
          id: Date.now().toString(),
          sender: 'bot',
          text: 'Sorry, the response is taking longer than expected. Please try again.',
          isError: true
        }]);
      }
    }, 15000); // 15 seconds timeout

    try {
      // Try to send message to API
      let response;
      let usedFallback = false;

      try {
        response = await apiClient.post('/api/Chatbot/Query', {
          query: messageText,
          employeeId: user?.employeeId,
          conversationId: conversationId
        });
      } catch (apiError) {
        console.log('API error, using fallback service:', apiError);
        // If API call fails, use the fallback service
        const fallbackResponse = fallbackChatbotService.processQuery(messageText);
        response = { data: fallbackResponse };
        usedFallback = true;
      }

      // Clear the timeout since we got a response
      clearTimeout(timeoutId);

      if (response.data) {
        // Save conversation ID for future messages
        if (!conversationId) {
          setConversationId(response.data.conversationId);
        }

        // Add bot response to chat
        const botMessage = {
          id: Date.now().toString(),
          sender: 'bot',
          text: response.data.response,
          intentName: response.data.intentName,
          confidence: response.data.confidence,
          isLocalResponse: usedFallback
        };

        setMessages(prev => [...prev, botMessage]);

        // Handle navigation if provided
        if (response.data.routeDestination) {
          setTimeout(() => {
            navigate(response.data.routeDestination);
            // Don't close the chatbot to show the navigation confirmation
          }, 1000);
        }

        // Handle API endpoint if provided
        if (response.data.apiEndpoint) {
          try {
            const apiResponse = await apiClient.get(response.data.apiEndpoint);
            if (apiResponse.data) {
              const apiResultMessage = {
                id: Date.now().toString() + '-api',
                sender: 'bot',
                text: typeof apiResponse.data === 'string'
                  ? apiResponse.data
                  : JSON.stringify(apiResponse.data, null, 2),
                isApiResult: true
              };
              setMessages(prev => [...prev, apiResultMessage]);
            }
          } catch (apiErr) {
            console.error('Error calling API endpoint:', apiErr);
          }
        }
      }
    } catch (err) {
      console.error('Error sending message to chatbot:', err);

      // Clear the timeout
      clearTimeout(timeoutId);

      // Determine the appropriate error message
      let errorMessage = 'Sorry, I\'m having trouble connecting to the server. Please try again later.';

      if (err.response) {
        // The request was made and the server responded with a status code outside of 2xx
        if (err.response.status === 404) {
          errorMessage = 'The chatbot service is currently unavailable. Please try again later.';
        } else if (err.response.status >= 500) {
          errorMessage = 'The server encountered an error. Our team has been notified.';
        }
      } else if (err.request) {
        // The request was made but no response was received
        errorMessage = 'No response from the server. Please check your internet connection.';
      }

      setMessages(prev => [...prev, {
        id: Date.now().toString(),
        sender: 'bot',
        text: errorMessage,
        isError: true
      }]);
    } finally {
      setIsProcessing(false);
    }
  };

  const handleVoiceInput = () => {
    if (recognition) {
      if (isRecording) {
        recognition.stop();
        setIsRecording(false);
      } else {
        setIsRecording(true);
        recognition.start();
      }
    } else {
      alert('Speech recognition is not supported in your browser.');
    }
  };

  const clearMessages = () => {
    setMessages([
      { id: 'welcome', sender: 'bot', text: 'Hello! I\'m your WorkNest assistant. How can I help you today?' }
    ]);
    setConversationId(null);
  };

  // Common suggestions based on user role
  const getSuggestions = () => {
    const commonSuggestions = [
      "What can you help me with?",
      "How do I request leave?",
      "Show my attendance history"
    ];

    const roleSuggestions = user?.role === 'Admin'
      ? ["Add a new employee", "Show department reports", "Manage leave policies"]
      : ["How many leaves do I have left?", "When is the next holiday?", "Update my profile"];

    return [...commonSuggestions, ...roleSuggestions];
  };

  return (
    <ChatbotContext.Provider
      value={{
        isOpen,
        messages,
        inputText,
        isProcessing,
        isRecording,
        openChatbot,
        closeChatbot,
        toggleChatbot,
        handleInputChange,
        handleSendMessage,
        handleVoiceInput,
        clearMessages,
        getSuggestions
      }}
    >
      {children}
    </ChatbotContext.Provider>
  );
};

export default ChatbotContext;
