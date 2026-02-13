// src/hooks/useFixedTasks.ts
import { useState, useEffect, useCallback } from 'react';
import { notification } from 'antd';
import dayjs from 'dayjs';
import {
    fetchFixedTasks,
    createFixedTask,
    updateFixedTask,
    deleteFixedTask,
    type FixedTaskReturnModel,
    type InputFixedTask,
} from '../api/fixedTasks';

export { postNewScheduleForTask } from '../api/fixedTasks';
import type { FixedTaskDisplayModel } from '../api/types';
import api from '../api/api';

const useFixedTasks = () => {
    const [tasks, setTasks] = useState<FixedTaskDisplayModel[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchTasks = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const fetchedTasks = await fetchFixedTasks();
            const mappedTasks: FixedTaskDisplayModel[] = fetchedTasks.map((task) => ({
                id: task.id,
                name: task.name,
                description: task.description,
                priority: task.priority,
                startTimestamp: dayjs(task.startTimestamp),
                endTimestamp: dayjs(task.endTimestamp),
            }));
            setTasks(mappedTasks);
        } catch (err) {
            const message = 'Failed to load tasks. Please check your network or API server connection.';
            setError(message);
            notification.error({ message: 'Error', description: message });
        } finally {
            setLoading(false);
        }
    }, []);

    /** Returns the new task id (Guid) when creating. */
    const createTask = useCallback(async (task: InputFixedTask): Promise<string> => {
        return await createFixedTask(task);
    }, []);

    const updateTask = useCallback(async (id: string, task: InputFixedTask) => {
        try {
            await updateFixedTask(id, task);
            await fetchTasks();
        } catch (err) {
            notification.error({ message: 'Error', description: 'Failed to update task.' });
        }
    }, [fetchTasks]);

    const deleteTask = useCallback(async (id: string) => {
        try {
            await deleteFixedTask(id);
            await fetchTasks();
        } catch (err) {
            notification.error({ message: 'Error', description: 'Failed to delete task.' });
        }
    }, [fetchTasks]);

    useEffect(() => {
        fetchTasks();
    }, [fetchTasks]);

    return { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask };
};

export default useFixedTasks;