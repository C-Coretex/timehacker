import api from './api';

export interface TaskForDayItem {
  isFixed: boolean;
  scheduleEntityId: string | null;
  task: {
    id: string;
    name: string;
    description: string | null;
    priority: number;
  };
  timeRange: {
    start: string;
    end: string;
  };
}

export interface TasksForDayResponse {
  date: string;
  tasksTimeline: TaskForDayItem[];
  categoriesTimeline: unknown[];
}

/** Format date as dd.MM.yyyy for API */
function formatDateForApi(d: Date): string {
  const day = String(d.getDate()).padStart(2, '0');
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const year = d.getFullYear();
  return `${day}.${month}.${year}`;
}

/** Parse C# TimeSpan or "HH:mm:ss" to minutes from midnight */
function parseTimeToMinutes(value: string): number {
  if (!value) return 0;
  // ISO 8601 duration (e.g. "PT1H30M") or "1.00:00:00" (days.hh:mm:ss) or "09:30:00"
  const isoMatch = value.match(/^PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?$/i);
  if (isoMatch) {
    const h = parseInt(isoMatch[1] ?? '0', 10);
    const m = parseInt(isoMatch[2] ?? '0', 10);
    const s = parseFloat(isoMatch[3] ?? '0');
    return h * 60 + m + s / 60;
  }
  // .NET TimeSpan format: "d.HH:mm:ss" (e.g. "0.09:30:00")
  const parts = value.split(/[.:]/).map(Number);
  if (parts.length >= 4) {
    const [days, hours, mins, secs] = parts;
    return (days ?? 0) * 24 * 60 + (hours ?? 0) * 60 + (mins ?? 0) + (secs ?? 0) / 60;
  }
  if (parts.length >= 3) {
    const [hours, mins, secs] = parts;
    return (hours ?? 0) * 60 + (mins ?? 0) + (secs ?? 0) / 60;
  }
  return 0;
}

export function minutesToDate(date: Date, minutesFromMidnight: number): Date {
  const d = new Date(date);
  d.setHours(0, 0, 0, 0);
  d.setMinutes(d.getMinutes() + minutesFromMidnight);
  return d;
}

/** Format date as YYYY-MM-DD for RefreshTasksForDays body */
function formatDateIso(d: Date): string {
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

export async function fetchTasksForDay(date: Date): Promise<TasksForDayResponse> {
  const dateStr = formatDateForApi(date);
  const response = await api.get<TasksForDayResponse>(`/api/Tasks/GetTasksForDay`, {
    params: { date: dateStr },
  });
  return response.data;
}

/** POST /api/Tasks/RefreshTasksForDays – refresh generated tasks for the given dates */
export async function refreshTasksForDays(dates: Date[]): Promise<void> {
  const body = dates.map(formatDateIso);
  await api.post('/api/Tasks/RefreshTasksForDays', body);
}

export function taskForDayToEvent(
  item: TaskForDayItem,
  date: Date,
  minutesToDateFn: (d: Date, minutes: number) => Date
): CalendarEvent {
  const startM = parseTimeToMinutes(item.timeRange.start);
  const endM = parseTimeToMinutes(item.timeRange.end);
  const start = minutesToDateFn(date, startM);
  const end = minutesToDateFn(date, endM);
  const task = item.task;
  return {
    id: `${task.id}-${item.scheduleEntityId ?? date.toISOString()}`,
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
