import { createContext, useContext, useState, useCallback, useMemo } from 'react';
import type { ReactNode, FC } from 'react';

export type TimeFormat = '12h' | '24h';
export type WeekStart = 'sunday' | 'monday';

interface SettingsContextType {
  timeFormat: TimeFormat;
  setTimeFormat: (format: TimeFormat) => void;
  weekStart: WeekStart;
  setWeekStart: (day: WeekStart) => void;
  timeDisplayFormat: string;
}

const SettingsContext = createContext<SettingsContextType | null>(null);

const TIME_FORMAT_KEY = 'time-format';
const WEEK_START_KEY = 'week-start';

const getStoredValue = <T extends string>(key: string, allowed: readonly T[], defaultValue: T): T => {
  try {
    const stored = localStorage.getItem(key);
    return stored && (allowed as readonly string[]).includes(stored) ? (stored as T) : defaultValue;
  } catch {
    return defaultValue;
  }
};

const TIME_FORMATS: readonly TimeFormat[] = ['12h', '24h'];
const WEEK_STARTS: readonly WeekStart[] = ['sunday', 'monday'];

interface SettingsProviderProps {
  children: ReactNode;
}

export const SettingsProvider: FC<SettingsProviderProps> = ({ children }) => {
  const [timeFormat, setTimeFormatState] = useState<TimeFormat>(() =>
    getStoredValue(TIME_FORMAT_KEY, TIME_FORMATS, '12h')
  );
  const [weekStart, setWeekStartState] = useState<WeekStart>(() =>
    getStoredValue(WEEK_START_KEY, WEEK_STARTS, 'sunday')
  );

  const setTimeFormat = useCallback((format: TimeFormat) => {
    localStorage.setItem(TIME_FORMAT_KEY, format);
    setTimeFormatState(format);
  }, []);

  const setWeekStart = useCallback((day: WeekStart) => {
    localStorage.setItem(WEEK_START_KEY, day);
    setWeekStartState(day);
  }, []);

  const timeDisplayFormat = useMemo(
    () => (timeFormat === '24h' ? 'HH:mm' : 'h:mm A'),
    [timeFormat]
  );

  const value = useMemo(
    () => ({ timeFormat, setTimeFormat, weekStart, setWeekStart, timeDisplayFormat }),
    [timeFormat, setTimeFormat, weekStart, setWeekStart, timeDisplayFormat]
  );

  return <SettingsContext.Provider value={value}>{children}</SettingsContext.Provider>;
};

export const useSettings = (): SettingsContextType => {
  const context = useContext(SettingsContext);
  if (!context) {
    throw new Error('useSettings must be used within SettingsProvider');
  }
  return context;
};
