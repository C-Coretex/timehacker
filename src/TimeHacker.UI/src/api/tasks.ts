import api from './api';
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

export interface TasksForDayResponse {
  date: string;
  tasksTimeline: TaskForDayItem[];
  categoriesTimeline: unknown[];
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
