import React, { useState, useEffect, useRef } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import axios from 'axios';
import apiClient from '../../services/apiClient';
import { useAuth } from '../../contexts/AuthContext';
import './MoodChangerModal.css';

const MoodChangerModal = ({ isOpen, onClose }) => {
  const [userInput, setUserInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [response, setResponse] = useState(null);
  const [isAnonymous, setIsAnonymous] = useState(false);
  const inputRef = useRef(null);
  const { currentUser } = useAuth();
  const closeTimeoutRef = useRef(null);

  useEffect(() => {
    if (isOpen && inputRef.current) {
      inputRef.current.focus();
    }

    // Clear any existing timeout when component mounts or isOpen changes
    return () => {
      if (closeTimeoutRef.current) {
        clearTimeout(closeTimeoutRef.current);
      }
    };
  }, [isOpen]);

  useEffect(() => {
    // Auto-close the modal after response is shown
    if (response) {
      closeTimeoutRef.current = setTimeout(() => {
        handleClose();
      }, 8000); // 8 seconds
    }

    return () => {
      if (closeTimeoutRef.current) {
        clearTimeout(closeTimeoutRef.current);
      }
    };
  }, [response]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!userInput.trim()) return;
    
    setIsLoading(true);
    
    try {
      const payload = {
        input: userInput,
        employeeId: currentUser?.id,
        isAnonymous: isAnonymous
      };
      
      const { data } = await apiClient.post('/api/MoodChanger/AnalyzeMood', payload);
      setResponse(data);
    } catch (error) {
      console.error('Error analyzing mood:', error);
      setResponse({
        mood: 'Neutral',
        response: "Thanks for sharing. You're not alone—we care about you.",
        backgroundColor: '#F0F8FF',
        emoji: '😐',
        id: 'error'
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setUserInput('');
    setResponse(null);
    if (closeTimeoutRef.current) {
      clearTimeout(closeTimeoutRef.current);
    }
    onClose();
  };

  const handleFeelBetter = () => {
    handleClose();
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div 
          className="mood-changer-overlay"
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.3 }}
        >
          <motion.div 
            className="mood-changer-modal"
            initial={{ scale: 0.8, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.8, opacity: 0 }}
            transition={{ duration: 0.3 }}
            style={{ 
              backgroundColor: response ? response.backgroundColor : '#ffffff'
            }}
          >
            <button className="mood-changer-close-btn" onClick={handleClose}>×</button>
            
            <div className="mood-changer-content">
              {!response ? (
                <>
                  <h2 className="mood-changer-title">🧠 How was your morning?</h2>
                  <form onSubmit={handleSubmit}>
                    <textarea
                      ref={inputRef}
                      className="mood-changer-input"
                      value={userInput}
                      onChange={(e) => setUserInput(e.target.value)}
                      placeholder="Share how you're feeling today..."
                      rows={4}
                    />
                    <div className="mood-changer-anonymous">
                      <input
                        type="checkbox"
                        id="anonymous"
                        checked={isAnonymous}
                        onChange={() => setIsAnonymous(!isAnonymous)}
                      />
                      <label htmlFor="anonymous">Submit anonymously</label>
                    </div>
                    <button 
                      type="submit" 
                      className="mood-changer-submit-btn"
                      disabled={isLoading || !userInput.trim()}
                    >
                      {isLoading ? 'Analyzing...' : 'Share'}
                    </button>
                  </form>
                </>
              ) : (
                <div className="mood-changer-response">
                  <div className="mood-changer-emoji">{response.emoji}</div>
                  <p className="mood-changer-message">{response.response}</p>
                  <button 
                    className="mood-changer-feel-better-btn"
                    onClick={handleFeelBetter}
                  >
                    Thanks, I feel better!
                  </button>
                </div>
              )}
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
};

export default MoodChangerModal;
