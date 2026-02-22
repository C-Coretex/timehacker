import { useCallback } from 'react';
import dayjs from 'dayjs';
import {
  fetchFixedTasks,
  createFixedTask,
  updateFixedTask,
  deleteFixedTask,
} from '../api/fixedTasks';
import type { FixedTaskDisplayModel, InputFixedTask } from '../api/types';
import { useEntityCrud } from './useEntityCrud';

export { postNewScheduleForTask } from '../api/fixedTasks';

const useFixedTasks = () => {
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
        tags: task.tags ?? [],
      }));
    },
    fetchErrorMessage: 'Failed to load tasks. Please check your network or API server connection.',
  });

  const createTask = useCallback(
    async (task: InputFixedTask): Promise<string> => createFixedTask(task),
    []
  );

  const updateTask = useCallback(
    async (id: string, task: InputFixedTask) => {
      await withRefetch(() => updateFixedTask(id, task), 'Failed to update task.');
    },
    [withRefetch]
  );

  const deleteTask = useCallback(
    async (id: string) => {
      await withRefetch(() => deleteFixedTask(id), 'Failed to delete task.');
    },
    [withRefetch]
  );

  return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

export default useFixedTasks;
