import { useMemo, useCallback } from 'react';
import type { CalendarView } from 'contexts/CalendarDateContext';

/**
 * Computes the set of dates covered by each calendar view (month/week/day/3-day)
 * for a given selected date and week-start preference.
 */
export function useCalendarDateRanges(selectedDate: Date, weekStartDay: number) {
  const weekStart = useMemo(() => {
    const d = new Date(selectedDate);
    const diff = (d.getDay() - weekStartDay + 7) % 7;
    d.setDate(d.getDate() - diff);
    d.setHours(0, 0, 0, 0);
    return d;
  }, [selectedDate, weekStartDay]);

  const weekDays = useMemo(
    () =>
      Array.from({ length: 7 }, (_, i) => {
        const d = new Date(weekStart);
        d.setDate(d.getDate() + i);
        return d;
      }),
    [weekStart]
  );

  const monthDays = useMemo(() => {
    const year = selectedDate.getFullYear();
    const month = selectedDate.getMonth();
    const lastDay = new Date(year, month + 1, 0).getDate();
    return Array.from({ length: lastDay }, (_, i) => new Date(year, month, i + 1));
  }, [selectedDate]);

  const dayDates = useMemo(() => {
    const d = new Date(selectedDate);
    d.setHours(0, 0, 0, 0);
    return [d];
  }, [selectedDate]);

  const threeDayDates = useMemo(
    () =>
      Array.from({ length: 3 }, (_, i) => {
        const d = new Date(selectedDate);
        d.setHours(0, 0, 0, 0);
        d.setDate(d.getDate() + i);
        return d;
      }),
    [selectedDate]
  );

  const getDatesForView = useCallback(
    (v: CalendarView): Date[] => {
      switch (v) {
        case 'month': return monthDays;
        case 'week': return weekDays;
        case 'day': return dayDates;
        case '3day': return threeDayDates;
        default: return weekDays;
      }
    },
    [monthDays, weekDays, dayDates, threeDayDates]
  );

  return { getDatesForView };
}
