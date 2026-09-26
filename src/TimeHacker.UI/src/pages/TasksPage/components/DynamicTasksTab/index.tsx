import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Table, Typography } from 'antd';
import { PlusCircleFilled } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useDynamicTasks } from '../../../../hooks/useDynamicTasks';
import { UnifiedTaskFormModal } from '../../../../components/UnifiedTaskFormModal';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../../../../api/types';
import { getDynamicTaskColumns } from './columns';
import { DynamicTaskCards } from './DynamicTaskCards';

export const DynamicTasksTab: FC = () => {
  const { t } = useTranslation();
  const { tasks, loading, error, createTask, updateTask, deleteTask } = useDynamicTasks();
  const { notification, modal } = App.useApp();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<DynamicTaskReturnModel | null>(null);

  const openModal = useCallback((task: DynamicTaskReturnModel | null) => {
    setEditingTask(task);
    setModalOpen(true);
  }, []);

  const handleDelete = useCallback((id: string) => {
    modal.confirm({
      title: t('tasks.confirmDelete'),
      content: t('tasks.confirmDeleteMessage'),
      okText: t('tasks.delete'),
      okType: 'danger',
      onOk: () => deleteTask(id),
    });
  }, [deleteTask, modal, t]);

  const handleSave = useCallback(
    async (data: InputDynamicTask, id?: string) => {
      const saved = id ? await updateTask(id, data) : await createTask(data);
      if (!saved) return;
      notification.success({
        title: t('tasks.success'),
        description: id ? t('tasks.dynamicTaskUpdated') : t('tasks.dynamicTaskAdded'),
      });
      setModalOpen(false);
    },
    [createTask, updateTask, notification, t]
  );

  return (
    <>
      <div style={{ marginBottom: '1rem' }}>
        <Button type="primary" icon={<PlusCircleFilled />} iconPlacement="end" onClick={() => openModal(null)}>
          {t('tasks.addDynamicTask')}
        </Button>
      </div>

      {error && (
        <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
          {error}
        </Typography.Text>
      )}

      <Table
        className="th-desktop-only"
        columns={getDynamicTaskColumns(t, openModal, handleDelete)}
        dataSource={tasks}
        loading={loading}
        rowKey="id"
        locale={{ emptyText: t('tasks.noDynamicTasks') }}
        scroll={{ x: 'max-content' }}
        size="middle"
      />
      <DynamicTaskCards className="th-phone-only" tasks={tasks} loading={loading} onEdit={openModal} onDelete={handleDelete} />

      <UnifiedTaskFormModal
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onSaveFixed={() => {}}
        onSaveDynamic={handleSave}
        initialDynamicData={editingTask}
        initialTab="dynamic"
      />
    </>
  );
};
