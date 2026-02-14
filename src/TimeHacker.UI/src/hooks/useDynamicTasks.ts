import { useState, useEffect, useCallback } from 'react';
import { notification } from 'antd';
import {
  fetchDynamicTasks,
  createDynamicTask,
  updateDynamicTask,
  deleteDynamicTask,
} from '../api/dynamicTasks';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../api/types';

const useDynamicTasks = () => {
  const [tasks, setTasks] = useState<DynamicTaskReturnModel[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const fetched = await fetchDynamicTasks();
      setTasks(fetched);
    } catch (err) {
      const message =
        'Failed to load dynamic tasks. Please check your network or API server connection.';
      setError(message);
      notification.error({ message: 'Error', description: message });
    } finally {
      setLoading(false);
    }
  }, []);

  const createTask = useCallback(async (task: InputDynamicTask) => {
    await createDynamicTask(task);
  }, []);

  const updateTask = useCallback(
    async (id: string, task: InputDynamicTask) => {
      try {
        await updateDynamicTask(id, task);
        await fetchTasks();
      } catch {
        notification.error({ message: 'Error', description: 'Failed to update task.' });
      }
    },
    [fetchTasks]
  );

  const deleteTask = useCallback(
    async (id: string) => {
      try {
        await deleteDynamicTask(id);
        await fetchTasks();
      } catch {
        notification.error({ message: 'Error', description: 'Failed to delete task.' });
      }
    },
    [fetchTasks]
  );

  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

export default useDynamicTasks;
