import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Table, Typography } from 'antd';
import { PlusCircleFilled } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useFixedTasks } from '../../../../hooks/useFixedTasks';
import { UnifiedTaskFormModal } from '../../../../components/UnifiedTaskFormModal';
import type { ScheduleFormPayload } from '../../../../components/UnifiedTaskFormModal';
import type { FixedTaskDisplayModel, FixedTaskFormData } from '../../../../api/types';
import { toFixedTaskPayload } from '../../../../utils/fixedTaskPayload';
import { getFixedTaskColumns } from './columns';
import { FixedTaskCards } from './FixedTaskCards';

export const FixedTasksTab: FC = () => {
  const { t } = useTranslation();
  const { tasks, loading, error, createTask, updateTask, deleteTask } = useFixedTasks();
  const { notification, modal } = App.useApp();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<FixedTaskDisplayModel | null>(null);

  const openModal = useCallback((task: FixedTaskDisplayModel | null) => {
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
    async (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => {
      const payload = toFixedTaskPayload(data);
      const saved = id ? await updateTask(id, payload) : (await createTask(payload, schedule)) !== null;
      if (!saved) return;
      notification.success({
        title: t('tasks.success'),
        description: id ? t('tasks.fixedTaskUpdated') : t('tasks.fixedTaskAdded'),
      });
      setModalOpen(false);
    },
    [createTask, updateTask, notification, t]
  );

  return (
    <>
      <div style={{ marginBottom: '1rem' }}>
        <Button type="primary" icon={<PlusCircleFilled />} iconPlacement="end" onClick={() => openModal(null)}>
          {t('tasks.addFixedTask')}
        </Button>
      </div>

      {error && (
        <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
          {error}
        </Typography.Text>
      )}

      <Table
        className="th-desktop-only"
        columns={getFixedTaskColumns(t, openModal, handleDelete)}
        dataSource={tasks}
        loading={loading}
        rowKey="id"
        locale={{ emptyText: t('tasks.noFixedTasks') }}
        scroll={{ x: 'max-content' }}
        size="middle"
      />
      <FixedTaskCards className="th-phone-only" tasks={tasks} loading={loading} onEdit={openModal} onDelete={handleDelete} />

      <UnifiedTaskFormModal
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onSaveFixed={handleSave}
        onSaveDynamic={() => {}}
        initialFixedData={editingTask ?? undefined}
        initialTab="fixed"
      />
    </>
  );
};
