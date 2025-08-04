// src/api/fixedTasks.ts
import axios from 'axios';
import type { FixedTaskReturnModel, InputFixedTask } from './types';

const API_BASE_URL = 'https://localhost:8081/api/FixedTasks';

// Create Axios instance with credentials
const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true, // Send cookies with requests
});

// Get all tasks
export const fetchFixedTasks = async (): Promise<FixedTaskReturnModel[]> => {
    const response = await apiClient.get('/GetAll');
    return response.data;
};

// Get task by ID
export const fetchFixedTaskById = async (id: string): Promise<FixedTaskReturnModel> => {
    const response = await apiClient.get(`/GetById/${id}`);
    return response.data;
};

// Create task
export const createFixedTask = async (task: InputFixedTask): Promise<FixedTaskReturnModel> => {
    const response = await apiClient.post('/Add', task);
    return response.data;
};

// Update task
export const updateFixedTask = async (id: string, task: InputFixedTask): Promise<void> => {
    await apiClient.put(`/Update/${id}`, task);
};

// Delete task
export const deleteFixedTask = async (id: string): Promise<void> => {
    await apiClient.delete(`/Delete/${id}`);
};