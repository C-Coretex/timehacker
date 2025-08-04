// src/pages/TasksPage.tsx
import React, { useCallback } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Typography } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import moment from 'moment';

import useFixedTasks from '../hooks/useFixedTasks';
import TaskFormModal, { type FixedTaskFormData } from '../components/TaskFormModal';
import type { FixedTaskDisplayModel } from '../types';

const { Title } = Typography;

// Columns for the tasks table
const columns = [
    {
        title: 'Name',
        dataIndex: 'name',
        key: 'name',
    },
    {
        title: 'Description',
        dataIndex: 'description',
        key: 'description',
    },
    {
        title: 'Priority',
        dataIndex: 'priority',
        key: 'priority',
    },
    {
        title: 'Start',
        dataIndex: 'startTimestamp',
        key: 'startTimestamp',
        render: (date: moment.Moment) => date.format('YYYY-MM-DD HH:mm'),
    },
    {
        title: 'End',
        dataIndex: 'endTimestamp',
        key: 'endTimestamp',
        render: (date: moment.Moment) => date.format('YYYY-MM-DD HH:mm'),
    },
    {
        title: 'Actions',
        key: 'actions',
        render: (_: unknown, task: FixedTaskDisplayModel) => (
            <>
                <Button
                    type="link"
                    icon={<EditOutlined />}
                    onClick={() => showEditTaskModal(task)}
                >
                    Edit
                </Button>
                <Button
                    type="link"
                    danger
                    icon={<DeleteOutlined />}
                    onClick={() => handleDeleteTask(task.id)}
                >
                    Delete
                </Button>
            </>
        ),
    },
];

// Tasks page component to display and manage fixed tasks
const TasksPage: FC = () => {
    const { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask } =
        useFixedTasks();
    const [isModalOpen, setIsModalOpen] = React.useState(false);
    const [editingTask, setEditingTask] = React.useState<FixedTaskDisplayModel | null>(null);

    // Open modal for adding a new task
    const showAddTaskModal = useCallback(() => {
        setEditingTask(null);
        setIsModalOpen(true);
    }, []);

    // Open modal for editing an existing task
    const showEditTaskModal = useCallback((task: FixedTaskDisplayModel) => {
        setEditingTask(task);
        setIsModalOpen(true);
    }, []);

    // Close modal and reset editing task
    const handleModalCancel = useCallback(() => {
        setIsModalOpen(false);
        setEditingTask(null);
    }, []);

    // Save task (create or update)
    const handleSaveTask = useCallback(
        async (taskData: FixedTaskFormData, id?: string) => {
            try {
                const apiInput = {
                    name: taskData.name,
                    description: taskData.description,
                    priority: taskData.priority,
                    startTimestamp: taskData.startTimestamp.toISOString(),
                    endTimestamp: taskData.endTimestamp.toISOString(),
                };

                if (id) {
                    await updateTask(id, apiInput);
                    notification.success({ message: 'Success', description: 'Task updated!' });
                } else {
                    await createTask(apiInput);
                    notification.success({ message: 'Success', description: 'Task added!' });
                }
                fetchTasks();
            } catch (error) {
                notification.error({
                    message: 'Error',
                    description: `Failed to ${id ? 'update' : 'add'} task.`,
                });
            } finally {
                handleModalCancel();
            }
        },
        [createTask, updateTask, fetchTasks, handleModalCancel]
    );

    // Delete task with confirmation
    const handleDeleteTask = useCallback(
        (id: string) => {
            Modal.confirm({
                title: 'Confirm Delete',
                content: 'Are you sure you want to delete this task?',
                okText: 'Delete',
                okType: 'danger',
                onOk: async () => {
                    try {
                        await deleteTask(id);
                        notification.success({ message: 'Success', description: 'Task deleted!' });
                        fetchTasks();
                    } catch (error) {
                        notification.error({ message: 'Error', description: 'Failed to delete task.' });
                    }
                },
            });
        },
        [deleteTask, fetchTasks]
    );

    return (
        <div style={{ padding: '1rem' }}>
            <Title level={2}>All Tasks</Title>
            <Button
                type="primary"
                icon={<PlusOutlined />}
                onClick={showAddTaskModal}
                style={{ marginBottom: '1rem' }}
            >
                Add Task
            </Button>
            {error && <Typography.Text type="danger">{error}</Typography.Text>}
            <Table
                columns={columns}
                dataSource={tasks}
                loading={loading}
                rowKey="id"
                locale={{ emptyText: 'No tasks created yet.' }}
            />
            <TaskFormModal
                open={isModalOpen}
                onCancel={handleModalCancel}
                onSave={handleSaveTask}
                initialData={editingTask}
            />
        </div>
    );
};

export default TasksPage;