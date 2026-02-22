import { createContext, useContext, useState } from 'react';
import type { FC, ReactNode } from 'react';

export type CalendarView = 'month' | 'week' | 'day' | '3day';

interface CalendarDateContextType {
  selectedDate: Date;
  setSelectedDate: (date: Date) => void;
  calendarView: CalendarView;
  setCalendarView: (view: CalendarView) => void;
}

const CalendarDateContext = createContext<CalendarDateContextType | null>(null);

export const CalendarDateProvider: FC<{ children: ReactNode }> = ({ children }) => {
  const [selectedDate, setSelectedDate] = useState(() => new Date());
  const [calendarView, setCalendarView] = useState<CalendarView>('week');

  return (
    <CalendarDateContext.Provider value={{ selectedDate, setSelectedDate, calendarView, setCalendarView }}>
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
