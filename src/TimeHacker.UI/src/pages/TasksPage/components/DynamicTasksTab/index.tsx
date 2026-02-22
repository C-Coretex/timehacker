import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { Button, Modal, notification, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import useDynamicTasks from '../../../../hooks/useDynamicTasks';
import UnifiedTaskFormModal from '../../../../components/UnifiedTaskFormModal';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../../../../api/types';
import { useIsMobile } from '../../../../hooks/useIsMobile';
import { getDynamicTaskColumns } from './columns';

const DynamicTasksTab: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { tasks, loading, error, createTask, updateTask, deleteTask } = useDynamicTasks();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<DynamicTaskReturnModel | null>(null);

  const openAddModal = useCallback(() => {
    setEditingTask(null);
    setModalOpen(true);
  }, []);

  const openEditModal = useCallback((task: DynamicTaskReturnModel) => {
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
    async (data: InputDynamicTask, id?: string) => {
      try {
        if (id) {
          await updateTask(id, data);
          notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskUpdated') });
        } else {
          await createTask(data);
          notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskAdded') });
        }
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.dynamicTaskSaveFailed') });
      } finally {
        closeModal();
      }
    },
    [createTask, updateTask, closeModal, t]
  );

  const columns = getDynamicTaskColumns(isMobile, t, openEditModal, handleDelete);

  return (
    <>
      <div style={{ marginBottom: '1rem' }}>
        <Button type="primary" icon={<PlusOutlined />} onClick={openAddModal} size={isMobile ? 'small' : 'middle'}>
          {t('tasks.addDynamicTask')}
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
        locale={{ emptyText: t('tasks.noDynamicTasks') }}
        scroll={isMobile ? { x: 500 } : undefined}
        size={isMobile ? 'small' : 'middle'}
      />

      <UnifiedTaskFormModal
        open={modalOpen}
        onCancel={closeModal}
        onSaveFixed={() => {}}
        onSaveDynamic={handleSave}
        initialDynamicData={editingTask}
        initialTab="dynamic"
      />
    </>
  );
};

export default DynamicTasksTab;
