import { createContext, useState, useEffect } from 'react';
import type { ReactNode, Dispatch, SetStateAction } from 'react';

const DARK_MODE = 'dark-mode';

type ThemeContextType = {
  darkMode: boolean;
  updateDarkMode: Dispatch<SetStateAction<boolean>>;
};

const ThemeContext = createContext<ThemeContextType>({
  darkMode: false,
  updateDarkMode: () => {},
});

type ThemeProviderProps = {
  children: ReactNode;
};

const ThemeProvider = ({ children }: ThemeProviderProps) => {
  const getInitialDarkMode = (): boolean => {
    const savedDarkMode = localStorage.getItem(DARK_MODE);
    return savedDarkMode ? JSON.parse(savedDarkMode) : false;
  };

  const [darkMode, setDarkMode] = useState<boolean>(getInitialDarkMode);

  useEffect(() => {
    localStorage.setItem(DARK_MODE, JSON.stringify(darkMode));
  }, [darkMode]);

  return (
    <ThemeContext.Provider value={{ darkMode, updateDarkMode: setDarkMode }}>
      {children}
    </ThemeContext.Provider>
  );
};

export { ThemeContext, ThemeProvider };
