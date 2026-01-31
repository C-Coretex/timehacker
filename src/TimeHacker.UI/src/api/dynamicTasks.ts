import type { DynamicTaskReturnModel, InputDynamicTask } from './types';
import api from './api';

const API_BASE_URL = '/api/DynamicTasks';


function mapToApi(task: InputDynamicTask): Record<string, unknown> {
  return {
    Name: task.name,
    Description: task.description ?? null,
    CategoryIds: task.categoryIds ?? [],
    Priority: task.priority,
    MinTimeToFinish: task.minTimeToFinish,
    MaxTimeToFinish: task.maxTimeToFinish,
    OptimalTimeToFinish: task.optimalTimeToFinish ?? null,
  };
}

export async function fetchDynamicTasks(): Promise<DynamicTaskReturnModel[]> {
  const response = await api.get<DynamicTaskReturnModel[] | unknown>(`${API_BASE_URL}/GetAll`);
  const data = Array.isArray(response.data) ? response.data : [];
  return data as DynamicTaskReturnModel[];
}

export async function createDynamicTask(task: InputDynamicTask): Promise<void> {
  await api.post(`${API_BASE_URL}/Add`, mapToApi(task));
}

export async function updateDynamicTask(id: string, task: InputDynamicTask): Promise<void> {
  await api.put(`${API_BASE_URL}/Update/${id}`, mapToApi(task));
}

export async function deleteDynamicTask(id: string): Promise<void> {
  await api.delete(`${API_BASE_URL}/Delete/${id}`);
}
