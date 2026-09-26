import { createContext } from 'react';
import dayjs from 'dayjs';
import type { CalendarEvent } from 'utils/calendarUtils';

/** Keys MonthEventsContext's map, one entry per calendar day. */
export const monthDayKey = (date: Date): string => dayjs(date).format('YYYY-MM-DD');

/**
 * The planner's tasks grouped by day, for MonthDateHeader's phone dots. Provided by PlannerCalendar — rbc builds
 * the month's date headers itself, so a prop cannot reach them.
 */
export const MonthEventsContext = createContext<ReadonlyMap<string, CalendarEvent[]>>(new Map());
