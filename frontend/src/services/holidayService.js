import apiClient from './apiClient';

// Get all holidays
export const getHolidays = async () => {
  try {
    const response = await apiClient.get('/holidays');
    return response.data;
  } catch (error) {
    console.error('Error fetching holidays:', error);
    throw error;
  }
};

// Get holidays for a specific year
export const getHolidaysByYear = async (year) => {
  try {
    const response = await apiClient.get(`/holidays/year/${year}`);
    return response.data;
  } catch (error) {
    console.error(`Error fetching holidays for year ${year}:`, error);
    throw error;
  }
};

// Get upcoming holidays (next 90 days)
export const getUpcomingHolidays = async () => {
  try {
    const response = await apiClient.get('/holidays/upcoming');
    return response.data.map(holiday => ({
      ...holiday,
      date: new Date(holiday.date) // Convert string dates to Date objects
    }));
  } catch (error) {
    console.error('Error fetching upcoming holidays:', error);
    // Return mock data if API fails
    const today = new Date();
    const nextMonth = new Date(today);
    nextMonth.setMonth(today.getMonth() + 1);
    
    return [
      {
        id: '1',
        name: 'Good Friday',
        date: new Date(2025, 3, 18), // April 18, 2025
        type: 'Company',
        isRecurringYearly: true
      },
      {
        id: '2',
        name: 'Labor Day',
        date: new Date(2025, 4, 1), // May 1, 2025
        type: 'Government',
        isRecurringYearly: true
      }
    ];
  }
};

// Format holidays for calendar display
export const formatHolidaysForCalendar = (holidays) => {
  return holidays.map(holiday => ({
    id: holiday.id,
    title: holiday.name,
    start: new Date(holiday.date),
    end: new Date(holiday.date),
    allDay: true,
    type: holiday.type,
    description: holiday.description,
    recurring: holiday.isRecurringYearly
  }));
};
