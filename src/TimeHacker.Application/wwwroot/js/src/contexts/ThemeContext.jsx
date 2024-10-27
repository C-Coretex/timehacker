import React, { createContext, useState, useEffect } from 'react';

const DARK_MODE = "dark-mode";

// Create a context
const ThemeContext = createContext();

// Create a provider component
const ThemeProvider = ({ children }) => {
  const getInitialDarkMode = () => {
    // Check local storage for the dark mode setting
    const savedDarkMode = localStorage.getItem(DARK_MODE);
    return savedDarkMode ? JSON.parse(savedDarkMode) : false;
  };

  const [darkMode, setDarkMode] = useState(getInitialDarkMode);

  useEffect(() => {
    // Save the dark mode setting to local storage whenever it changes
    localStorage.setItem(DARK_MODE, JSON.stringify(darkMode));
  }, [darkMode]);

  const updateDarkMode = (mode) => {
    setDarkMode(mode);
  };

  return (
    <ThemeContext.Provider value={{ darkMode, updateDarkMode }}>
      {children}
    </ThemeContext.Provider>
  );
};

export { ThemeContext, ThemeProvider };