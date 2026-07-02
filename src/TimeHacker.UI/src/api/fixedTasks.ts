// src/api/fixedTasks.ts
import type {
    FixedTaskReturnModel,
    InputFixedTask,
    InputScheduleEntityModel,
} from './types';
import { api } from './api';

const API_BASE_URL = '/api/fixed-tasks';
const TASKS_API_URL = '/api/tasks';

export const fetchFixedTasks = async (): Promise<FixedTaskReturnModel[]> => {
    const response = await api.get(`${API_BASE_URL}`);
    return response.data;
};

export const fetchFixedTaskById = async (id: string): Promise<FixedTaskReturnModel> => {
    const response = await api.get(`${API_BASE_URL}/${id}`);
    return response.data;
};

/** Add fixed task. Returns the new task's Id (Guid). */
export const createFixedTask = async (task: InputFixedTask): Promise<string> => {
    const response = await api.post<string>(`${API_BASE_URL}`, task);
    return response.data;
};

/** Post new schedule for a fixed task (repeating entity). Call after createFixedTask with the returned id. */
export const postNewScheduleForTask = async (
    body: InputScheduleEntityModel
): Promise<unknown> => {
    const response = await api.post(`${TASKS_API_URL}/schedules`, body);
    return response.data;
};

export const updateFixedTask = async (id: string, task: InputFixedTask): Promise<void> => {
    await api.put(`${API_BASE_URL}/${id}`, task);
};

export const deleteFixedTask = async (id: string): Promise<void> => {
    await api.delete(`${API_BASE_URL}/${id}`);
};
