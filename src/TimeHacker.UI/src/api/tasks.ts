import { api } from './api';
import { formatDateIso } from '../utils/timeUtils';

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

/** A category's time window generated for one day — the backdrop tasks sit on top of. */
export interface CategoryForDayItem {
  scheduleEntityId: string | null;
  /** The window this band came from; a category may contribute several on one day. */
  categoryScheduleId: string;
  /** The window's own optional note; the band is labelled by the category's name. */
  scheduleDescription: string | null;
  /** The parent category, which is what tasks link to. */
  category: {
    id: string;
    name: string;
    description: string | null;
    color: number;
  };
  timeRange: {
    start: string;
    end: string;
  };
}

export interface TasksForDayResponse {
  date: string;
  tasksTimeline: TaskForDayItem[];
  categoriesTimeline: CategoryForDayItem[];
}

export async function fetchTasksForDay(date: Date): Promise<TasksForDayResponse> {
  const response = await api.get<TasksForDayResponse>('/api/tasks/timeline/day', {
    params: { date: formatDateIso(date) },
  });
  return response.data;
}

export async function fetchTasksForDays(dates: Date[]): Promise<TasksForDayResponse[]> {
  const params = new URLSearchParams();
  for (const d of dates) {
    params.append('dates', formatDateIso(d));
  }
  const response = await api.get<TasksForDayResponse[]>('/api/tasks/timeline', { params });
  return response.data;
}

export async function refreshTasksForDays(dates: Date[]): Promise<void> {
  const body = dates.map(formatDateIso);
  await api.post('/api/tasks/timeline/refresh', body);
}
