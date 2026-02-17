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

/** Parse C# TimeSpan or "HH:mm:ss" to minutes from midnight.
 *  .NET formats: "HH:mm:ss", "d.HH:mm:ss", "HH:mm:ss.fffffff", "d.HH:mm:ss.fffffff" */
function parseTimeToMinutes(value: string): number {
  if (!value) return 0;
  // ISO 8601 duration (e.g. "PT1H30M")
  const isoMatch = value.match(/^PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?$/i);
  if (isoMatch) {
    const h = parseInt(isoMatch[1] ?? '0', 10);
    const m = parseInt(isoMatch[2] ?? '0', 10);
    const s = parseFloat(isoMatch[3] ?? '0');
    return h * 60 + m + s / 60;
  }
  // .NET TimeSpan: distinguish "d.HH:mm:ss[.fff]" from "HH:mm:ss[.fff]"
  // "d.HH:mm:ss" has a dot BEFORE the first colon; "HH:mm:ss.fff" has dot AFTER colons
  const daysMatch = value.match(/^(\d+)\.(\d+):(\d+):(\d+)/);
  if (daysMatch) {
    const days = parseInt(daysMatch[1], 10);
    const hours = parseInt(daysMatch[2], 10);
    const mins = parseInt(daysMatch[3], 10);
    const secs = parseInt(daysMatch[4], 10);
    return days * 24 * 60 + hours * 60 + mins + secs / 60;
  }
  // "HH:mm:ss" or "HH:mm:ss.fffffff"
  const timeMatch = value.match(/^(\d+):(\d+):(\d+)/);
  if (timeMatch) {
    const hours = parseInt(timeMatch[1], 10);
    const mins = parseInt(timeMatch[2], 10);
    const secs = parseInt(timeMatch[3], 10);
    return hours * 60 + mins + secs / 60;
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

/** GET /api/Tasks/GetTasksForDays – fetch tasks for multiple dates in one request */
export async function fetchTasksForDays(dates: Date[]): Promise<TasksForDayResponse[]> {
  const params = new URLSearchParams();
  for (const d of dates) {
    params.append('dates', formatDateIso(d));
  }
  const response = await api.get<TasksForDayResponse[]>('/api/Tasks/GetTasksForDays', { params });
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
  minutesToDateFn: (d: Date, minutes: number) => Date,
  index?: number
): CalendarEvent {
  const startM = parseTimeToMinutes(item.timeRange.start);
  const endM = parseTimeToMinutes(item.timeRange.end);
  const start = minutesToDateFn(date, startM);
  const end = minutesToDateFn(date, endM);
  const task = item.task;
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
