import { useCallback } from 'react';
import {
  fetchDynamicTasks,
  createDynamicTask,
  updateDynamicTask,
  deleteDynamicTask,
} from '../api/dynamicTasks';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../api/types';
import { useEntityCrud } from './useEntityCrud';

const useDynamicTasks = () => {
  const { items: tasks, loading, error, fetch: fetchTasks, withRefetch } = useEntityCrud<DynamicTaskReturnModel>({
    fetchFn: fetchDynamicTasks,
    fetchErrorMessage: 'Failed to load dynamic tasks. Please check your network or API server connection.',
  });

  const createTask = useCallback(
    async (task: InputDynamicTask): Promise<void> => { await createDynamicTask(task); },
    []
  );

  const updateTask = useCallback(
    async (id: string, task: InputDynamicTask) => {
      await withRefetch(() => updateDynamicTask(id, task), 'Failed to update task.');
    },
    [withRefetch]
  );

  const deleteTask = useCallback(
    async (id: string) => {
      await withRefetch(() => deleteDynamicTask(id), 'Failed to delete task.');
    },
    [withRefetch]
  );

  return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

export default useDynamicTasks;
