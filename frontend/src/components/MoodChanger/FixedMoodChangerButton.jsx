import React, { useState } from 'react';
import { motion } from 'framer-motion';
import MoodChangerModal from './MoodChangerModal';
import './MoodChangerButton.css';

const FixedMoodChangerButton = () => {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const openModal = () => {
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
  };

  return (
    <>
      <motion.button
        className="fixed-mood-changer-button"
        onClick={openModal}
        whileHover={{ scale: 1.05 }}
        whileTap={{ scale: 0.95 }}
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3 }}
        style={{
          position: 'fixed',
          bottom: '20px',
          right: '20px',
          zIndex: 999,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          gap: '8px',
          background: 'linear-gradient(135deg, #8e44ad, #9b59b6)',
          color: 'white',
          border: 'none',
          padding: '12px 20px',
          borderRadius: '50px',
          fontWeight: 600,
          cursor: 'pointer',
          boxShadow: '0 4px 15px rgba(0, 0, 0, 0.2)',
          transition: 'all 0.3s ease'
        }}
      >
        <span className="mood-changer-icon">🧠</span>
        <span className="mood-changer-text">MoodChanger</span>
      </motion.button>
      
      <MoodChangerModal isOpen={isModalOpen} onClose={closeModal} />
    </>
  );
};

export default FixedMoodChangerButton;
