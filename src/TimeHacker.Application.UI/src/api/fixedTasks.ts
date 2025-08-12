// src/api/fixedTasks.ts
import type { FixedTaskReturnModel, InputFixedTask } from './types';
import api from './api';

const API_BASE_URL = '/api/FixedTasks';

export const fetchFixedTasks = async (): Promise<FixedTaskReturnModel[]> => {
    const response = await api.get(`${API_BASE_URL}/GetAll`);
    return response.data;
};

export const fetchFixedTaskById = async (id: string): Promise<FixedTaskReturnModel> => {
    const response = await api.get(`${API_BASE_URL}/GetById/${id}`);
    return response.data;
};

export const createFixedTask = async (task: InputFixedTask): Promise<FixedTaskReturnModel> => {
    const response = await api.post(`${API_BASE_URL}/Add`, task);
    return response.data;
};

export const updateFixedTask = async (id: string, task: InputFixedTask): Promise<void> => {
    await api.put(`${API_BASE_URL}/Update/${id}`, task);
};

export const deleteFixedTask = async (id: string): Promise<void> => {
    await api.delete(`${API_BASE_URL}/Delete/${id}`);
};
