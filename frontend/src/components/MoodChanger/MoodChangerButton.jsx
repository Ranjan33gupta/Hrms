import React, { useState } from 'react';
import { motion } from 'framer-motion';
import MoodChangerModal from './MoodChangerModal';
import './MoodChangerButton.css';

const MoodChangerButton = () => {
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
        className="mood-changer-button"
        onClick={openModal}
        whileHover={{ scale: 1.05 }}
        whileTap={{ scale: 0.95 }}
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3 }}
      >
        <span className="mood-changer-icon">🧠</span>
        <span className="mood-changer-text">MoodChanger</span>
      </motion.button>
      
      <MoodChangerModal isOpen={isModalOpen} onClose={closeModal} />
    </>
  );
};

export default MoodChangerButton;
