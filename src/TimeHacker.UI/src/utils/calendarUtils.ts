import type { CategoryForDayItem, TaskForDayItem } from '../api/tasks';
import { parseTimeToMinutes, utcMinutesToDate, localMinutesToDate } from './timeUtils';

export interface TaskEventResource {
  type: 'fixed' | 'dynamic';
  isFixed: boolean;
  task: { id: string; name: string; description: string | null; priority: number };
  start: Date;
  end: Date;
}

export interface CategoryEventResource {
  type: 'category';
  /** The parent category — its colour drives the band, and it is what tasks link to. */
  category: { id: string; name: string; description: string | null; color: number };
  /** The window this band came from, and its own optional note. */
  categoryScheduleId: string;
  scheduleDescription: string | null;
  scheduleEntityId: string | null;
  /**
   * How many other category windows already overlap this one on the same day. Drives the left inset of
   * the colour rail so stacked windows each stay visible instead of hiding one another.
   */
  depth: number;
  start: Date;
  end: Date;
}

export type CalendarEventResource = TaskEventResource | CategoryEventResource;

export interface CalendarEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  allDay: boolean;
  description?: string;
  resource?: CalendarEventResource;
}

export function taskForDayToEvent(item: TaskForDayItem, date: Date, index?: number): CalendarEvent {
  const startM = parseTimeToMinutes(item.timeRange.start);
  const endM = parseTimeToMinutes(item.timeRange.end);
  const start = utcMinutesToDate(date, startM);
  const end = utcMinutesToDate(date, endM);
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

/**
 * Converts one day's category windows into background events.
 *
 * Unlike tasks, category times are wall-clock (`TimeOnly`, never UTC-converted), so they anchor to local
 * midnight — see `localMinutesToDate`. Overlap depth is assigned here, per day, because several categories
 * may legitimately cover the same hour and each still has to be identifiable.
 */
export function categoriesForDayToEvents(items: CategoryForDayItem[], date: Date): CalendarEvent[] {
  const windows = items
    .map((item) => ({
      item,
      startM: parseTimeToMinutes(item.timeRange.start),
      endM: parseTimeToMinutes(item.timeRange.end),
    }))
    .sort((a, b) => a.startM - b.startM || b.endM - a.endM);

  // Each window's depth is how many earlier (outer) windows it still sits inside.
  const openEnds: number[] = [];

  return windows.map(({ item, startM, endM }, index) => {
    while (openEnds.length > 0 && openEnds[openEnds.length - 1] <= startM) openEnds.pop();
    const depth = openEnds.length;
    openEnds.push(endM);

    const start = localMinutesToDate(date, startM);
    const end = localMinutesToDate(date, endM);

    return {
      // Keyed on the window, not the category: one category may contribute several bands to a day,
      // and two of them can share a start time.
      id: `category-schedule-${item.categoryScheduleId}-${date.toISOString()}-${index}`,
      title: item.category.name,
      start,
      end,
      allDay: false,
      // The category's description; the window's own note is surfaced separately by the detail modal.
      description: item.category.description ?? undefined,
      resource: {
        type: 'category' as const,
        category: item.category,
        categoryScheduleId: item.categoryScheduleId,
        scheduleDescription: item.scheduleDescription,
        scheduleEntityId: item.scheduleEntityId,
        depth,
        start,
        end,
      },
    };
  });
}
