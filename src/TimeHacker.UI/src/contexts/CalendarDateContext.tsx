import { createContext, useContext, useState } from 'react';
import type { FC, ReactNode } from 'react';

interface CalendarDateContextType {
  selectedDate: Date;
  setSelectedDate: (date: Date) => void;
}

const CalendarDateContext = createContext<CalendarDateContextType | null>(null);

export const CalendarDateProvider: FC<{ children: ReactNode }> = ({ children }) => {
  const [selectedDate, setSelectedDate] = useState(() => new Date());

  return (
    <CalendarDateContext.Provider value={{ selectedDate, setSelectedDate }}>
      {children}
    </CalendarDateContext.Provider>
  );
};

export const useCalendarDate = (): CalendarDateContextType => {
  const context = useContext(CalendarDateContext);
  if (!context) {
    throw new Error('useCalendarDate must be used within a CalendarDateProvider');
  }
  return context;
};
