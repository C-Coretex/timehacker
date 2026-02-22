import type { TaskForDayItem } from '../api/tasks';
import { parseTimeToMinutes, minutesToDate } from './timeUtils';

export interface CalendarEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  allDay: boolean;
  description?: string;
  resource?: {
    type: 'fixed' | 'dynamic';
    isFixed: boolean;
    task: { id: string; name: string; description: string | null; priority: number };
    start: Date;
    end: Date;
  };
}

export function taskForDayToEvent(item: TaskForDayItem, date: Date, index?: number): CalendarEvent {
  const startM = parseTimeToMinutes(item.timeRange.start);
  const endM = parseTimeToMinutes(item.timeRange.end);
  const start = minutesToDate(date, startM);
  const end = minutesToDate(date, endM);
  const { task } = item;
  return {
    id: `${task.id}-${date.toISOString()}-${item.timeRange.start}-${index ?? 0}`,
    title: task.name,
    start,
    end,
    allDay: false,
    description: task.description ?? undefined,
    resource: {
      type: item.isFixed ? 'fixed' : 'dynamic',
      isFixed: item.isFixed,
      task: {
        id: task.id,
        name: task.name,
        description: task.description,
        priority: task.priority,
      },
      start,
      end,
    },
  };
}
