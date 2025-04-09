/**
 * Utility functions for handling dates in the application
 */

/**
 * Converts a date to UTC and returns it as an ISO string
 * @param {Date|string} date - The date to convert
 * @returns {string|null} - The UTC date as an ISO string, or null if invalid
 */
export const toUtcIsoString = (date) => {
  if (!date) return null;
  
  try {
    // Create a date object from the input
    const dateObj = date instanceof Date ? date : new Date(date);
    
    // Ensure the date is valid
    if (isNaN(dateObj.getTime())) {
      console.warn('Invalid date:', date);
      return null;
    }
    
    // Explicitly convert to UTC
    const utcYear = dateObj.getUTCFullYear();
    const utcMonth = dateObj.getUTCMonth();
    const utcDate = dateObj.getUTCDate();
    const utcHours = dateObj.getUTCHours();
    const utcMinutes = dateObj.getUTCMinutes();
    const utcSeconds = dateObj.getUTCSeconds();
    
    // Create a new date object in UTC
    const utcDateObj = new Date(Date.UTC(utcYear, utcMonth, utcDate, utcHours, utcMinutes, utcSeconds));
    
    // Return as ISO string
    return utcDateObj.toISOString();
  } catch (error) {
    console.error('Error converting date to UTC:', error);
    return null;
  }
};

/**
 * Creates a UTC date at noon to avoid timezone boundary issues
 * @param {Date|string} date - The date to convert
 * @returns {string|null} - The UTC date at noon as an ISO string, or null if invalid
 */
export const toUtcNoonIsoString = (date) => {
  if (!date) return null;
  
  try {
    // Create a date object from the input
    const dateObj = date instanceof Date ? date : new Date(date);
    
    // Ensure the date is valid
    if (isNaN(dateObj.getTime())) {
      console.warn('Invalid date:', date);
      return null;
    }
    
    // Create a UTC date at noon to avoid timezone issues
    const utcDateObj = new Date(Date.UTC(
      dateObj.getFullYear(),
      dateObj.getMonth(),
      dateObj.getDate(),
      12, 0, 0 // noon UTC
    ));
    
    // Return as ISO string
    return utcDateObj.toISOString();
  } catch (error) {
    console.error('Error converting date to UTC noon:', error);
    return null;
  }
};

/**
 * Formats a date for display in the UI
 * @param {Date|string} date - The date to format
 * @param {Object} options - The formatting options
 * @returns {string} - The formatted date string
 */
export const formatDateForDisplay = (date, options = {}) => {
  if (!date) return '';
  
  try {
    const dateObj = date instanceof Date ? date : new Date(date);
    
    // Ensure the date is valid
    if (isNaN(dateObj.getTime())) {
      return String(date);
    }
    
    // Default options
    const defaultOptions = { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    };
    
    // Merge with provided options
    const formattingOptions = { ...defaultOptions, ...options };
    
    return dateObj.toLocaleDateString('en-US', formattingOptions);
  } catch (error) {
    console.error('Error formatting date for display:', error);
    return String(date);
  }
};

/**
 * Parses a date string from an input field
 * @param {string} dateString - The date string from an input field
 * @returns {Date|null} - The parsed Date object, or null if invalid
 */
export const parseDateInput = (dateString) => {
  if (!dateString) return null;
  
  try {
    // For date inputs in format YYYY-MM-DD
    if (/^\d{4}-\d{2}-\d{2}$/.test(dateString)) {
      const [year, month, day] = dateString.split('-').map(Number);
      // Note: month is 0-indexed in JavaScript Date
      return new Date(year, month - 1, day);
    }
    
    // For other formats, use the Date constructor
    const date = new Date(dateString);
    return isNaN(date.getTime()) ? null : date;
  } catch (error) {
    console.error('Error parsing date input:', error);
    return null;
  }
};

/**
 * Formats a date for an input field (YYYY-MM-DD)
 * @param {Date|string} date - The date to format
 * @returns {string} - The formatted date string (YYYY-MM-DD)
 */
export const formatDateForInput = (date) => {
  if (!date) return '';
  
  try {
    const dateObj = date instanceof Date ? date : new Date(date);
    
    // Ensure the date is valid
    if (isNaN(dateObj.getTime())) {
      return '';
    }
    
    const year = dateObj.getFullYear();
    // Month is 0-indexed, so add 1 and pad with leading zero if needed
    const month = String(dateObj.getMonth() + 1).padStart(2, '0');
    const day = String(dateObj.getDate()).padStart(2, '0');
    
    return `${year}-${month}-${day}`;
  } catch (error) {
    console.error('Error formatting date for input:', error);
    return '';
  }
};
