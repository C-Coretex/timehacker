import { api } from './api';
import type { CategoryReturnModel, InputCategory, InputScheduleEntityModel, ScheduleEntityReturnModel } from './types';

const API_BASE_URL = '/api/categories';

export const fetchCategories = async (): Promise<CategoryReturnModel[]> => {
  const response = await api.get<CategoryReturnModel[] | unknown>(`${API_BASE_URL}`);
  const data = Array.isArray(response.data) ? response.data : [];
  return data as CategoryReturnModel[];
};

export const fetchCategoryById = async (id: string): Promise<CategoryReturnModel> => {
  const response = await api.get<CategoryReturnModel>(`${API_BASE_URL}/${id}`);
  return response.data;
};

/** Add category. Returns the new category's Id (Guid). */
export const createCategory = async (category: InputCategory): Promise<string> => {
  const response = await api.post<string>(`${API_BASE_URL}`, category);
  return response.data;
};

/**
 * Attach a recurrence to a category. Call after createCategory with the returned id — a category with no
 * schedule never lands on a day, so this is what puts it on the calendar.
 */
export const postNewScheduleForCategory = async (
  body: InputScheduleEntityModel
): Promise<ScheduleEntityReturnModel> => {
  const response = await api.post<ScheduleEntityReturnModel>(`${API_BASE_URL}/schedules`, body);
  return response.data;
};

export const updateCategory = async (id: string, category: InputCategory): Promise<void> => {
  await api.put(`${API_BASE_URL}/${id}`, category);
};

export const deleteCategory = async (id: string): Promise<void> => {
  await api.delete(`${API_BASE_URL}/${id}`);
};
