import React, { useCallback } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Tabs, Typography } from 'antd';
import type { Breakpoint } from 'antd';
import { PlusOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

import useFixedTasks, { postNewScheduleForTask } from '../hooks/useFixedTasks';
import useDynamicTasks from '../hooks/useDynamicTasks';
import TaskFormModal from '../components/TaskFormModal';
import type { ScheduleFormPayload } from '../components/TaskFormModal';
import DynamicTaskFormModal from '../components/DynamicTaskFormModal';
import type { FixedTaskFormData, FixedTaskDisplayModel, DynamicTaskReturnModel, InputDynamicTask } from '../api/types';
import { useIsMobile } from '../hooks/useIsMobile';

const { Title } = Typography;

const TasksPage: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
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
    { title: t('tasks.name'), dataIndex: 'name', key: 'name' },
    { title: t('tasks.description'), dataIndex: 'description', key: 'description', responsive: ['md'] as Breakpoint[] },
    { title: t('tasks.priority'), dataIndex: 'priority', key: 'priority', width: isMobile ? 60 : undefined },
    {
      title: isMobile ? t('tasks.start') : t('tasks.startTime'),
      dataIndex: 'startTimestamp',
      key: 'startTimestamp',
      render: (date: Dayjs) => date.format(isMobile ? 'MM/DD HH:mm' : 'YYYY-MM-DD HH:mm'),
    },
    {
      title: isMobile ? t('tasks.end') : t('tasks.endTime'),
      dataIndex: 'endTimestamp',
      key: 'endTimestamp',
      render: (date: Dayjs) => date.format(isMobile ? 'MM/DD HH:mm' : 'YYYY-MM-DD HH:mm'),
    },
    {
      title: t('tasks.actions'),
      key: 'actions',
      width: isMobile ? 80 : undefined,
      render: (_: unknown, task: FixedTaskDisplayModel) => (
        <>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => showEditFixedModal(task)}
            size={isMobile ? 'small' : 'middle'}
          >
            {!isMobile && t('tasks.edit')}
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => setDeleteModal({ visible: true, id: task.id, type: 'fixed' })}
            size={isMobile ? 'small' : 'middle'}
          >
            {!isMobile && t('tasks.delete')}
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
    { title: t('tasks.name'), dataIndex: 'name', key: 'name' },
    { title: t('tasks.description'), dataIndex: 'description', key: 'description', responsive: ['md'] as Breakpoint[] },
    { title: t('tasks.priority'), dataIndex: 'priority', key: 'priority', width: isMobile ? 60 : undefined },
    {
      title: isMobile ? t('tasks.min') : t('tasks.minDuration'),
      dataIndex: 'minTimeToFinish',
      key: 'minTimeToFinish',
      render: formatDuration,
    },
    {
      title: isMobile ? t('tasks.max') : t('tasks.maxDuration'),
      dataIndex: 'maxTimeToFinish',
      key: 'maxTimeToFinish',
      render: formatDuration,
    },
    {
      title: isMobile ? t('tasks.optimal') : t('tasks.optimalDuration'),
      dataIndex: 'optimalTimeToFinish',
      key: 'optimalTimeToFinish',
      render: (v: string | null) => (v ? formatDuration(v) : '-'),
      responsive: ['md'] as Breakpoint[],
    },
    {
      title: t('tasks.actions'),
      key: 'actions',
      width: isMobile ? 80 : undefined,
      render: (_: unknown, task: DynamicTaskReturnModel) => (
        <>
          <Button
            type="link"
            icon={<EditOutlined />}
            onClick={() => showEditDynamicModal(task)}
            size={isMobile ? 'small' : 'middle'}
          >
            {!isMobile && t('tasks.edit')}
          </Button>
          <Button
            type="link"
            danger
            icon={<DeleteOutlined />}
            onClick={() => setDeleteModal({ visible: true, id: task.id, type: 'dynamic' })}
            size={isMobile ? 'small' : 'middle'}
          >
            {!isMobile && t('tasks.delete')}
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
          notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskUpdated') });
        } else {
          const newId = await createFixedTask(payload);
          if (schedule) {
            await postNewScheduleForTask({
              parentEntityId: newId,
              repeatingEntityType: schedule.repeatingEntityType,
              endsOnModel: schedule.endsOnModel ?? undefined,
            });
          }
          notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskAdded') });
        }
        await fetchFixedTasks();
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.fixedTaskSaveFailed') });
      } finally {
        handleFixedModalCancel();
      }
    },
    [createFixedTask, updateFixedTask, fetchFixedTasks, handleFixedModalCancel, t]
  );

  const handleSaveDynamicTask = useCallback(
    async (data: InputDynamicTask, id?: string) => {
      try {
        if (id) {
          await updateDynamicTask(id, data);
          notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskUpdated') });
        } else {
          await createDynamicTask(data);
          notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskAdded') });
        }
        await fetchDynamicTasks();
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.dynamicTaskSaveFailed') });
      } finally {
        handleDynamicModalCancel();
      }
    },
    [createDynamicTask, updateDynamicTask, fetchDynamicTasks, handleDynamicModalCancel, t]
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
        <Title level={isMobile ? 4 : 2} style={{ margin: 0 }}>
          {t('tasks.allTasks')}
        </Title>
      </div>

      <Tabs
        items={[
          {
            key: 'fixed',
            label: t('tasks.fixedTasks'),
            children: (
              <>
                <div style={{ marginBottom: '1rem' }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={showAddFixedModal} size={isMobile ? 'small' : 'middle'}>
                    {t('tasks.addFixedTask')}
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
                  locale={{ emptyText: t('tasks.noFixedTasks') }}
                  scroll={isMobile ? { x: 500 } : undefined}
                  size={isMobile ? 'small' : 'middle'}
                />
              </>
            ),
          },
          {
            key: 'dynamic',
            label: t('tasks.dynamicTasks'),
            children: (
              <>
                <div style={{ marginBottom: '1rem' }}>
                  <Button type="primary" icon={<PlusOutlined />} onClick={showAddDynamicModal} size={isMobile ? 'small' : 'middle'}>
                    {t('tasks.addDynamicTask')}
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
                  locale={{ emptyText: t('tasks.noDynamicTasks') }}
                  scroll={isMobile ? { x: 500 } : undefined}
                  size={isMobile ? 'small' : 'middle'}
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
        initialData={editingFixedTask ?? undefined}
      />
      <DynamicTaskFormModal
        open={dynamicModalOpen}
        onCancel={handleDynamicModalCancel}
        onSave={handleSaveDynamicTask}
        initialData={editingDynamicTask}
      />
      <Modal
        open={deleteModal.visible}
        title={t('tasks.confirmDelete')}
        okText={t('tasks.delete')}
        okType="danger"
        onOk={confirmDelete}
        onCancel={() => setDeleteModal({ visible: false })}
      >
        {t('tasks.confirmDeleteMessage')}
      </Modal>
    </div>
  );
};

export default TasksPage;
