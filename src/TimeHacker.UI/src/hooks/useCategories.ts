import { useCallback } from 'react';
import dayjs from 'dayjs';
import {
  fetchCategories,
  createCategory,
  updateCategory,
  deleteCategory,
} from '../api/categories';
import type {
  CategoryDisplayModel,
  CategoryScheduleReturnModel,
  InputCategory,
} from '../api/types';
import { useEntityCrud } from './useEntityCrud';

export const toCategoryScheduleDisplay = (schedule: CategoryScheduleReturnModel) => ({
  id: schedule.id,
  categoryId: schedule.categoryId,
  description: schedule.description,
  date: dayjs(schedule.date),
  startTime: dayjs(schedule.startTime, 'HH:mm:ss'),
  endTime: dayjs(schedule.endTime, 'HH:mm:ss'),
  scheduleEntity: schedule.scheduleEntity ?? null,
});

export const useCategories = () => {
  const {
    items: categories,
    loading,
    error,
    fetch: fetchAll,
    withRefetch,
  } = useEntityCrud<CategoryDisplayModel>({
    fetchFn: async () => {
      const data = await fetchCategories();
      return data.map((category) => ({
        id: category.id,
        name: category.name,
        description: category.description,
        color: category.color,
        // Schedules ride along on the category GET, so the list needs no extra request.
        schedules: (category.schedules ?? []).map(toCategoryScheduleDisplay),
      }));
    },
    fetchErrorMessage:
      'Failed to load categories. Please check your network or API server connection.',
  });

  const create = useCallback(
    async (category: InputCategory) => {
      await withRefetch(() => createCategory(category), 'Failed to create category.');
    },
    [withRefetch]
  );

  const update = useCallback(
    async (id: string, category: InputCategory) => {
      await withRefetch(() => updateCategory(id, category), 'Failed to update category.');
    },
    [withRefetch]
  );

  const remove = useCallback(
    async (id: string) => {
      await withRefetch(() => deleteCategory(id), 'Failed to delete category.');
    },
    [withRefetch]
  );

  return { categories, loading, error, fetchCategories: fetchAll, create, update, remove };
};
