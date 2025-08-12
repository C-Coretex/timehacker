import React, { useCallback } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Typography } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import moment from 'moment';

import useFixedTasks from '../hooks/useFixedTasks';
import TaskFormModal, { type FixedTaskFormData } from '../components/TaskFormModal';
import type { FixedTaskDisplayModel } from '../types';

const { Title } = Typography;

const TasksPage: FC = () => {
    const { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask } = useFixedTasks();
    const [isModalOpen, setIsModalOpen] = React.useState(false);
    const [editingTask, setEditingTask] = React.useState<FixedTaskDisplayModel | null>(null);

    const showAddTaskModal = useCallback(() => {
        setEditingTask(null);
        setIsModalOpen(true);
    }, []);

    const showEditTaskModal = useCallback((task: FixedTaskDisplayModel) => {
        setEditingTask(task);
        setIsModalOpen(true);
    }, []);

    const handleModalCancel = useCallback(() => {
        setIsModalOpen(false);
        setEditingTask(null);
    }, []);

    const columns = [
        { title: 'Name', dataIndex: 'name', key: 'name' },
        { title: 'Description', dataIndex: 'description', key: 'description' },
        { title: 'Priority', dataIndex: 'priority', key: 'priority' },
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
                    <Button type="link" icon={<EditOutlined />} onClick={() => showEditTaskModal(task)}>
                        Edit
                    </Button>
                    <Button type="link" danger icon={<DeleteOutlined />} onClick={() => handleDeleteTask(task.id)}>
                        Delete
                    </Button>
                </>
            ),
        },
    ];

    const handleSaveTask = useCallback(
        async (data: FixedTaskFormData, id?: string) => {
            try {
                const payload = {
                    name: data.name,
                    description: data.description,
                    priority: data.priority,
                    startTimestamp: data.startTimestamp?.toISOString() ?? '',
                    endTimestamp: data.endTimestamp?.toISOString() ?? '',
                };

                if (id) {
                    await updateTask(id, payload);
                    notification.success({ message: 'Success', description: 'Task updated!' });
                } else {
                    await createTask(payload);
                    notification.success({ message: 'Success', description: 'Task added!' });
                }
            } catch (error) {
                notification.error({ message: 'Error', description: `Failed to ${id ? 'update' : 'add'} task.` });
            } finally {
                handleModalCancel();
            }
        },
        [createTask, updateTask, handleModalCancel]
    );

    const handleDeleteTask = useCallback(
        (id: string) => {
            Modal.confirm({
                title: 'Confirm Delete',
                content: 'Are you sure you want to delete this task?',
                okText: 'Delete',
                okType: 'danger',
                onOk: async () => {
                    await deleteTask(id);
                },
            });
        },
        [deleteTask]
    );

    return (
        <div>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <Title level={2} style={{ margin: 0 }}>All Tasks</Title>
                <Button type="primary" icon={<PlusOutlined />} onClick={showAddTaskModal}>
                    Add Task
                </Button>
            </div>
            {error && <Typography.Text type="danger">{error}</Typography.Text>}
            <Table columns={columns} dataSource={tasks} loading={loading} rowKey="id" locale={{ emptyText: 'No tasks created yet.' }} />
            <TaskFormModal open={isModalOpen} onCancel={handleModalCancel} onSave={handleSaveTask} initialData={editingTask} />
        </div>
    );
};

export default TasksPage;