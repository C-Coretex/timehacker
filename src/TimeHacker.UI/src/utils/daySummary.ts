import dayjs from 'dayjs';
import { isTaskEvent } from './calendarUtils';
import type { CalendarEvent } from './calendarUtils';
import { isHighPriority } from './priority';

export interface DaySummary {
  planned: number;
  remaining: number;
  highPriorityRemaining: number;
}

/** One day's task counts, taken from events already loaded for the view — no extra request. */
export function summarizeDay(events: CalendarEvent[], day: Date, now: Date = new Date()): DaySummary {
  const tasks = events.filter(isTaskEvent).filter((event) => dayjs(event.start).isSame(day, 'day'));
  const remaining = tasks.filter((event) => event.end > now);

  return {
    planned: tasks.length,
    remaining: remaining.length,
    highPriorityRemaining: remaining.filter((event) => isHighPriority(event.resource.task.priority)).length,
  };
}
