import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import useFixedTasks, { postNewScheduleForTask } from '../../../../hooks/useFixedTasks';
import UnifiedTaskFormModal from '../../../../components/UnifiedTaskFormModal';
import type { ScheduleFormPayload } from '../../../../components/UnifiedTaskFormModal';
import type { FixedTaskDisplayModel, FixedTaskFormData } from '../../../../api/types';
import { useIsMobile } from '../../../../hooks/useIsMobile';
import { getFixedTaskColumns } from './columns';

const FixedTasksTab: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { tasks, loading, error, fetchTasks, createTask, updateTask, deleteTask } = useFixedTasks();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<FixedTaskDisplayModel | null>(null);

  const openAddModal = useCallback(() => {
    setEditingTask(null);
    setModalOpen(true);
  }, []);

  const openEditModal = useCallback((task: FixedTaskDisplayModel) => {
    setEditingTask(task);
    setModalOpen(true);
  }, []);

  const closeModal = useCallback(() => {
    setModalOpen(false);
    setEditingTask(null);
  }, []);

  const handleDelete = useCallback((id: string) => {
    Modal.confirm({
      title: t('tasks.confirmDelete'),
      content: t('tasks.confirmDeleteMessage'),
      okText: t('tasks.delete'),
      okType: 'danger',
      onOk: () => deleteTask(id),
    });
  }, [deleteTask, t]);

  const handleSave = useCallback(
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
          await updateTask(id, payload);
          notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskUpdated') });
        } else {
          const newId = await createTask(payload);
          if (schedule) {
            await postNewScheduleForTask({
              parentEntityId: newId,
              repeatingEntityType: schedule.repeatingEntityType,
              endsOnModel: schedule.endsOnModel ?? undefined,
            });
          }
          notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskAdded') });
          await fetchTasks();
        }
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.fixedTaskSaveFailed') });
      } finally {
        closeModal();
      }
    },
    [createTask, updateTask, fetchTasks, closeModal, t]
  );

  const columns = getFixedTaskColumns(isMobile, t, openEditModal, handleDelete);

  return (
    <>
      <div style={{ marginBottom: '1rem' }}>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal} size={isMobile ? 'small' : 'middle'}>
          {t('tasks.addFixedTask')}
        </Button>
      </div>

      {error && (
        <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
          {error}
        </Typography.Text>
      )}

      <Table
        columns={columns}
        dataSource={tasks}
        loading={loading}
        rowKey="id"
        locale={{ emptyText: t('tasks.noFixedTasks') }}
        scroll={isMobile ? { x: 500 } : undefined}
        size={isMobile ? 'small' : 'middle'}
      />

      <UnifiedTaskFormModal
        open={modalOpen}
        onCancel={closeModal}
        onSaveFixed={handleSave}
        onSaveDynamic={() => {}}
        initialFixedData={editingTask ?? undefined}
        initialTab="fixed"
      />
    </>
  );
};

export default FixedTasksTab;
