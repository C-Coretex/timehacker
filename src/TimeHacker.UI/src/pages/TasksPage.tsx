import React, { useCallback } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Tabs, Typography } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import moment from 'moment';

import useFixedTasks, { postNewScheduleForTask } from '../hooks/useFixedTasks';
import useDynamicTasks from '../hooks/useDynamicTasks';
import TaskFormModal, {
    type FixedTaskFormData,
    type ScheduleFormPayload,
} from '../components/TaskFormModal';
import DynamicTaskFormModal from '../components/DynamicTaskFormModal';
import type { FixedTaskDisplayModel, DynamicTaskReturnModel, InputDynamicTask } from '../api/types';

const { Title } = Typography;

const TasksPage: FC = () => {
  const {
    tasks: fixedTasks,
    loading: fixedLoading,
    error: fixedError,
    fetchTasks: fetchFixedTasks,
    createTask: createFixedTask,
    updateTask: updateFixedTask,
    deleteTask: deleteFixedTask,
  } = useFixedTasks();
  const {
    tasks: dynamicTasks,
    loading: dynamicLoading,
    error: dynamicError,
    fetchTasks: fetchDynamicTasks,
    createTask: createDynamicTask,
    updateTask: updateDynamicTask,
    deleteTask: deleteDynamicTask,
  } = useDynamicTasks();

  const [fixedModalOpen, setFixedModalOpen] = React.useState(false);
  const [editingFixedTask, setEditingFixedTask] = React.useState<FixedTaskDisplayModel | null>(null);
  const [dynamicModalOpen, setDynamicModalOpen] = React.useState(false);
  const [editingDynamicTask, setEditingDynamicTask] = React.useState<DynamicTaskReturnModel | null>(null);
  const [deleteModal, setDeleteModal] = React.useState<{
    visible: boolean;
    id?: string;
    type?: 'fixed' | 'dynamic';
  }>({ visible: false });

  const showAddFixedModal = useCallback(() => {
    setEditingFixedTask(null);
    setFixedModalOpen(true);
  }, []);

  const showEditFixedModal = useCallback((task: FixedTaskDisplayModel) => {
    setEditingFixedTask(task);
    setFixedModalOpen(true);
  }, []);

  const showAddDynamicModal = useCallback(() => {
    setEditingDynamicTask(null);
    setDynamicModalOpen(true);
  }, []);

  const showEditDynamicModal = useCallback((task: DynamicTaskReturnModel) => {
    setEditingDynamicTask(task);
    setDynamicModalOpen(true);
  }, []);

  const handleFixedModalCancel = useCallback(() => {
    setFixedModalOpen(false);
    setEditingFixedTask(null);
  }, []);

  const handleDynamicModalCancel = useCallback(() => {
    setDynamicModalOpen(false);
    setEditingDynamicTask(null);
  }, []);

  const fixedColumns = [
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
          <Button type="link" icon={<EditOutlined />} onClick={() => showEditFixedModal(task)}>
            Edit
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => setDeleteModal({ visible: true, id: task.id, type: 'fixed' })}
          >
            Delete
          </Button>
        </>
      ),
    },
  ];

  const formatDuration = (value: string): string => {
    if (!value) return '-';
    const parts = value.split(/[.:]/).map(Number);
    if (parts.length >= 3) {
      const [h, m] = parts;
      return `${h ?? 0}h ${m ?? 0}m`;
    }
    return value;
  };

  const dynamicColumns = [
    { title: 'Name', dataIndex: 'name', key: 'name' },
    { title: 'Description', dataIndex: 'description', key: 'description' },
    { title: 'Priority', dataIndex: 'priority', key: 'priority' },
    {
      title: 'Min duration',
      dataIndex: 'minTimeToFinish',
      key: 'minTimeToFinish',
      render: formatDuration,
    },
    {
      title: 'Max duration',
      dataIndex: 'maxTimeToFinish',
      key: 'maxTimeToFinish',
      render: formatDuration,
    },
    {
      title: 'Optimal duration',
      dataIndex: 'optimalTimeToFinish',
      key: 'optimalTimeToFinish',
      render: (v: string | null) => (v ? formatDuration(v) : '-'),
    },
    {
      title: 'Actions',
      key: 'actions',
      render: (_: unknown, task: DynamicTaskReturnModel) => (
        <>
          <Button type="link" icon={<EditOutlined />} onClick={() => showEditDynamicModal(task)}>
            Edit
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => setDeleteModal({ visible: true, id: task.id, type: 'dynamic' })}
          >
            Delete
          </Button>
        </>
      ),
    },
  ];

  const handleSaveFixedTask = useCallback(
    async (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => {
      try {
        const payload = {
          name: data.name,
          description: data.description,
          priority: data.priority,
          startTimestamp: data.startTimestamp?.toISOString() ?? '',
          endTimestamp: data.endTimestamp?.toISOString() ?? '',
        };
        if (id) {
          await updateFixedTask(id, payload);
          notification.success({ message: 'Success', description: 'Fixed task updated!' });
        } else {
          const newId = await createFixedTask(payload);
          if (schedule) {
            await postNewScheduleForTask({
              parentEntityId: newId,
              repeatingEntityType: schedule.repeatingEntityType,
              endsOnModel: schedule.endsOnModel ?? undefined,
            });
          }
          notification.success({ message: 'Success', description: 'Fixed task added!' });
        }
        await fetchFixedTasks();
      } catch {
        notification.error({ message: 'Error', description: 'Failed to save fixed task.' });
      } finally {
        handleFixedModalCancel();
      }
    },
    [createFixedTask, updateFixedTask, fetchFixedTasks, handleFixedModalCancel]
  );

  const handleSaveDynamicTask = useCallback(
    async (data: InputDynamicTask, id?: string) => {
      try {
        if (id) {
          await updateDynamicTask(id, data);
          notification.success({ message: 'Success', description: 'Dynamic task updated!' });
        } else {
          await createDynamicTask(data);
          notification.success({ message: 'Success', description: 'Dynamic task added!' });
        }
        await fetchDynamicTasks();
      } catch {
        notification.error({ message: 'Error', description: 'Failed to save dynamic task.' });
      } finally {
        handleDynamicModalCancel();
      }
    },
    [createDynamicTask, updateDynamicTask, fetchDynamicTasks, handleDynamicModalCancel]
  );

  const confirmDelete = async () => {
    if (!deleteModal.id || !deleteModal.type) return;
    if (deleteModal.type === 'fixed') {
      await deleteFixedTask(deleteModal.id);
      await fetchFixedTasks();
    } else {
      await deleteDynamicTask(deleteModal.id);
      await fetchDynamicTasks();
    }
    setDeleteModal({ visible: false });
  };

  return (
    <div>
      <div
        style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          marginBottom: '1rem',
        }}
      >
        <Title level={2} style={{ margin: 0 }}>
          All Tasks
        </Title>
      </div>

      <Tabs
        items={[
          {
            key: 'fixed',
            label: 'Fixed Tasks',
            children: (
              <>
                <div style={{ marginBottom: '1rem' }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={showAddFixedModal}>
                    Add Fixed Task
                  </Button>
                </div>
                {fixedError && (
                  <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
                    {fixedError}
                  </Typography.Text>
                )}
                <Table
                  columns={fixedColumns}
                  dataSource={fixedTasks}
                  loading={fixedLoading}
                  rowKey="id"
                  locale={{ emptyText: 'No fixed tasks yet.' }}
                />
              </>
            ),
          },
          {
            key: 'dynamic',
            label: 'Dynamic Tasks',
            children: (
              <>
                <div style={{ marginBottom: '1rem' }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={showAddDynamicModal}>
                    Add Dynamic Task
                  </Button>
                </div>
                {dynamicError && (
                  <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
                    {dynamicError}
                  </Typography.Text>
                )}
                <Table
                  columns={dynamicColumns}
                  dataSource={dynamicTasks}
                  loading={dynamicLoading}
                  rowKey="id"
                  locale={{ emptyText: 'No dynamic tasks yet.' }}
                />
              </>
            ),
          },
        ]}
      />

      <TaskFormModal
        open={fixedModalOpen}
        onCancel={handleFixedModalCancel}
        onSave={handleSaveFixedTask}
        initialData={editingFixedTask}
      />
      <DynamicTaskFormModal
        open={dynamicModalOpen}
        onCancel={handleDynamicModalCancel}
        onSave={handleSaveDynamicTask}
        initialData={editingDynamicTask}
      />
      <Modal
        open={deleteModal.visible}
        title="Confirm Delete"
        okText="Delete"
        okType="danger"
        onOk={confirmDelete}
        onCancel={() => setDeleteModal({ visible: false })}
      >
        Are you sure you want to delete this task?
      </Modal>
    </div>
  );
};

export default TasksPage;
