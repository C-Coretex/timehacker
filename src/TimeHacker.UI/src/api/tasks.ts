import api from './api';
import { formatDateForApi, formatDateIso } from '../utils/timeUtils';

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
  const response = await api.get<TasksForDayResponse>('/api/Tasks/GetTasksForDay', {
    params: { date: formatDateForApi(date) },
  });
  return response.data;
}

export async function fetchTasksForDays(dates: Date[]): Promise<TasksForDayResponse[]> {
  const params = new URLSearchParams();
  for (const d of dates) {
    params.append('dates', formatDateIso(d));
  }
  const response = await api.get<TasksForDayResponse[]>('/api/Tasks/GetTasksForDays', { params });
  return response.data;
}

export async function refreshTasksForDays(dates: Date[]): Promise<void> {
  const body = dates.map(formatDateIso);
  await api.post('/api/Tasks/RefreshTasksForDays', body);
}
