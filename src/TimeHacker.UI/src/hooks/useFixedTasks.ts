import { useCallback } from 'react';
import dayjs from 'dayjs';
import {
  fetchFixedTasks,
  createFixedTaskWithSchedule,
  updateFixedTask,
  deleteFixedTask,
} from '../api/fixedTasks';
import type { FixedTaskDisplayModel, InputFixedTask } from '../api/types';
import type { SchedulePayload } from '../utils/buildSchedulePayload';
import { useEntityCrud } from './useEntityCrud';

export const useFixedTasks = () => {
  const { items: tasks, loading, error, fetch: fetchTasks, withRefetch } = useEntityCrud<FixedTaskDisplayModel>({
    fetchFn: async () => {
      const data = await fetchFixedTasks();
      return data.map((task) => ({
        id: task.id,
        name: task.name,
        description: task.description,
        priority: task.priority,
        startTimestamp: dayjs(task.startTimestamp),
        endTimestamp: dayjs(task.endTimestamp),
        scheduleEntity: task.scheduleEntity ?? null,
        categories: task.categories ?? [],
        categoryIds: (task.categories ?? []).map((category) => category.id),
        tags: task.tags ?? [],
      }));
    },
    fetchErrorMessage: 'Failed to load tasks. Please check your network or API server connection.',
  });

  /** Creates the task and its optional recurrence; resolves to the new id, or null when that failed. */
  const createTask = useCallback(
    async (task: InputFixedTask, schedule?: SchedulePayload): Promise<string | null> => {
      const result = await withRefetch(() => createFixedTaskWithSchedule(task, schedule), 'Failed to create task.');
      return result.succeeded ? result.value : null;
    },
    [withRefetch]
  );

  const updateTask = useCallback(
    async (id: string, task: InputFixedTask): Promise<boolean> =>
      (await withRefetch(() => updateFixedTask(id, task), 'Failed to update task.')).succeeded,
    [withRefetch]
  );

  const deleteTask = useCallback(
    async (id: string): Promise<boolean> =>
      (await withRefetch(() => deleteFixedTask(id), 'Failed to delete task.')).succeeded,
    [withRefetch]
  );

  return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

