/**
 * Fallback Chatbot Service
 * Provides local responses when the backend chatbot service is unavailable
 */

// Simple intent recognition based on keywords
const recognizeIntent = (message) => {
  const text = message.toLowerCase();
  
  if (text.includes('hello') || text.includes('hi') || text.includes('hey')) {
    return { intent: 'greeting', confidence: 0.9 };
  }
  
  if (text.includes('leave') || text.includes('vacation') || text.includes('time off')) {
    return { intent: 'leave_request', confidence: 0.8 };
  }
  
  if (text.includes('attendance') || text.includes('clock in') || text.includes('clock out')) {
    return { intent: 'attendance', confidence: 0.8 };
  }
  
  if (text.includes('salary') || text.includes('pay') || text.includes('payroll')) {
    return { intent: 'payroll', confidence: 0.8 };
  }
  
  if (text.includes('profile') || text.includes('account') || text.includes('my info')) {
    return { intent: 'profile', confidence: 0.8 };
  }
  
  if (text.includes('help') || text.includes('support') || text.includes('assistance')) {
    return { intent: 'help', confidence: 0.8 };
  }
  
  return { intent: 'unknown', confidence: 0.5 };
};

// Generate responses based on intent
const generateResponse = (intent, message) => {
  switch (intent) {
    case 'greeting':
      return {
        response: "Hello! I'm your WorkNest assistant. How can I help you today?",
        intentName: 'greeting',
        confidence: 0.9
      };
      
    case 'leave_request':
      return {
        response: "To request leave, go to the 'Request Leave' section in the sidebar. You can select dates and provide a reason for your leave request.",
        intentName: 'leave_request',
        confidence: 0.8
      };
      
    case 'attendance':
      return {
        response: "You can view your attendance records in the 'Attendance' section. To clock in or out, use the buttons on your dashboard.",
        intentName: 'attendance',
        confidence: 0.8
      };
      
    case 'payroll':
      return {
        response: "Payroll information can be found in the 'Payroll' section. If you have specific questions about your salary, please contact HR.",
        intentName: 'payroll',
        confidence: 0.8
      };
      
    case 'profile':
      return {
        response: "You can view and update your profile information by clicking on your user icon in the top right and selecting 'Profile'.",
        intentName: 'profile',
        confidence: 0.8
      };
      
    case 'help':
      return {
        response: "I'm here to help! You can ask me about leave requests, attendance, payroll, or your profile. If you need more assistance, please contact the HR department.",
        intentName: 'help',
        confidence: 0.8
      };
      
    default:
      // Handle unknown intents with a more helpful response
      if (message.endsWith('?')) {
        return {
          response: "I'm not sure I understand your question. You can ask me about leave requests, attendance, payroll, or your profile information.",
          intentName: 'unknown',
          confidence: 0.5
        };
      } else {
        return {
          response: "I'm not sure how to help with that. You can ask me about leave requests, attendance, payroll, or your profile information.",
          intentName: 'unknown',
          confidence: 0.5
        };
      }
  }
};

// Main function to process a query
const processQuery = (message) => {
  const { intent, confidence } = recognizeIntent(message);
  const response = generateResponse(intent, message);
  
  return {
    ...response,
    conversationId: `local-${Date.now()}`,
    requiresAuth: false
  };
};

export default {
  processQuery
};
