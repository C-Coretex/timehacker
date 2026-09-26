import { useCallback } from 'react';
import {
  fetchDynamicTasks,
  createDynamicTask,
  updateDynamicTask,
  deleteDynamicTask,
} from '../api/dynamicTasks';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../api/types';
import { useEntityCrud } from './useEntityCrud';

export const useDynamicTasks = () => {
  const { items: tasks, loading, error, fetch: fetchTasks, withRefetch } = useEntityCrud<DynamicTaskReturnModel>({
    fetchFn: fetchDynamicTasks,
    fetchErrorMessage: 'Failed to load dynamic tasks. Please check your network or API server connection.',
  });

  const createTask = useCallback(
    async (task: InputDynamicTask): Promise<boolean> =>
      (await withRefetch(() => createDynamicTask(task), 'Failed to create task.')).succeeded,
    [withRefetch]
  );

  const updateTask = useCallback(
    async (id: string, task: InputDynamicTask): Promise<boolean> =>
      (await withRefetch(() => updateDynamicTask(id, task), 'Failed to update task.')).succeeded,
    [withRefetch]
  );

  const deleteTask = useCallback(
    async (id: string): Promise<boolean> =>
      (await withRefetch(() => deleteDynamicTask(id), 'Failed to delete task.')).succeeded,
    [withRefetch]
  );

  return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

