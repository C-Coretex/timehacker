import { api } from './api';
import type {
  CategoryScheduleReturnModel,
  InputCategorySchedule,
  InputScheduleEntityModel,
  ScheduleEntityReturnModel,
} from './types';

const API_BASE_URL = '/api/categories';

const schedulesUrl = (categoryId: string) => `${API_BASE_URL}/${categoryId}/schedules`;

/**
 * A category's own schedules also ride along on the category GET, so this is only needed when
 * refreshing one category in isolation.
 */
export const fetchCategorySchedules = async (
  categoryId: string
): Promise<CategoryScheduleReturnModel[]> => {
  const response = await api.get<CategoryScheduleReturnModel[] | unknown>(schedulesUrl(categoryId));
  const data = Array.isArray(response.data) ? response.data : [];
  return data as CategoryScheduleReturnModel[];
};

/** Add a time window to a category. Returns the new schedule's Id (Guid). */
export const createCategorySchedule = async (
  categoryId: string,
  schedule: InputCategorySchedule
): Promise<string> => {
  const response = await api.post<string>(schedulesUrl(categoryId), schedule);
  return response.data;
};

export const updateCategorySchedule = async (
  categoryId: string,
  id: string,
  schedule: InputCategorySchedule
): Promise<void> => {
  await api.put(`${schedulesUrl(categoryId)}/${id}`, schedule);
};

export const deleteCategorySchedule = async (categoryId: string, id: string): Promise<void> => {
  await api.delete(`${schedulesUrl(categoryId)}/${id}`);
};

/**
 * Attach a recurrence to a category schedule. Call after createCategorySchedule with the returned id —
 * the window already lands on its own date, and this repeats it on later days.
 * `parentEntityId` is the schedule's id, not the category's.
 */
export const postNewRecurrenceForCategorySchedule = async (
  body: InputScheduleEntityModel
): Promise<ScheduleEntityReturnModel> => {
  const response = await api.post<ScheduleEntityReturnModel>(`${API_BASE_URL}/schedules`, body);
  return response.data;
};
