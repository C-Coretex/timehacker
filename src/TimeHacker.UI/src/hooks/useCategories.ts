import { useCallback } from 'react';
import dayjs from 'dayjs';
import {
  fetchCategories,
  createCategory,
  updateCategory,
  deleteCategory,
} from '../api/categories';
import type { CategoryDisplayModel, InputCategory } from '../api/types';
import { useEntityCrud } from './useEntityCrud';

export { postNewScheduleForCategory } from '../api/categories';

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
        date: dayjs(category.date),
        startTime: dayjs(category.startTime, 'HH:mm:ss'),
        endTime: dayjs(category.endTime, 'HH:mm:ss'),
        scheduleEntity: category.scheduleEntity ?? null,
      }));
    },
    fetchErrorMessage:
      'Failed to load categories. Please check your network or API server connection.',
  });

  // Deliberately skips withRefetch: the caller needs the returned Guid to attach a schedule, and
  // refetches once both calls have completed (same contract as useFixedTasks.createTask).
  const create = useCallback(
    async (category: InputCategory): Promise<string> => createCategory(category),
    []
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
