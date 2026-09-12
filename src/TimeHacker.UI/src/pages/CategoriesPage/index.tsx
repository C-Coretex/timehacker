import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useCategories } from '../../hooks/useCategories';
import { useCategorySchedules } from '../../hooks/useCategorySchedules';
import { CategoryFormModal } from '../../components/CategoryFormModal';
import { CategoryScheduleFormModal } from '../../components/CategoryScheduleFormModal';
import type {
  CategoryDisplayModel,
  CategoryFormData,
  CategoryScheduleDisplayModel,
  CategoryScheduleFormData,
  InputCategory,
  InputCategorySchedule,
} from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';
import { useIsMobile } from '../../hooks/useIsMobile';
import { getCategoryColumns } from './columns';
import { SchedulesPanel } from './SchedulesPanel';

const toPayload = (data: CategoryFormData): InputCategory => ({
  name: data.name,
  description: data.description || undefined,
  color: data.color,
});

const toSchedulePayload = (data: CategoryScheduleFormData): InputCategorySchedule => ({
  description: data.description || undefined,
  date: data.date.format('YYYY-MM-DD'),
  startTime: data.startTime.format('HH:mm:ss'),
  endTime: data.endTime.format('HH:mm:ss'),
});

export const CategoriesPage: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { categories, loading, error, fetchCategories, create, update, remove } = useCategories();
  const schedules = useCategorySchedules({ onChanged: fetchCategories });
  const { notification, modal } = App.useApp();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryDisplayModel | null>(null);

  const [scheduleModalOpen, setScheduleModalOpen] = useState(false);
  const [scheduleParent, setScheduleParent] = useState<CategoryDisplayModel | null>(null);
  const [editingSchedule, setEditingSchedule] = useState<CategoryScheduleDisplayModel | null>(null);

  const openAddModal = useCallback(() => {
    setEditingCategory(null);
    setModalOpen(true);
  }, []);

  const openEditModal = useCallback((category: CategoryDisplayModel) => {
    setEditingCategory(category);
    setModalOpen(true);
  }, []);

  const closeModal = useCallback(() => {
    setModalOpen(false);
    setEditingCategory(null);
  }, []);

  const handleDelete = useCallback(
    (id: string) => {
      modal.confirm({
        title: t('categories.confirmDelete'),
        content: t('categories.confirmDeleteMessage'),
        okText: t('categories.delete'),
        okType: 'danger',
        onOk: () => remove(id),
      });
    },
    [remove, modal, t]
  );

  const handleSave = useCallback(
    async (data: CategoryFormData, id?: string) => {
      const payload = toPayload(data);
      if (id) {
        await update(id, payload);
        notification.success({
          title: t('categories.success'),
          description: t('categories.categoryUpdated'),
        });
      } else {
        await create(payload);
        notification.success({
          title: t('categories.success'),
          description: t('categories.categoryAdded'),
        });
      }
      closeModal();
    },
    [create, update, closeModal, notification, t]
  );

  const openAddScheduleModal = useCallback((category: CategoryDisplayModel) => {
    setScheduleParent(category);
    setEditingSchedule(null);
    setScheduleModalOpen(true);
  }, []);

  const openEditScheduleModal = useCallback(
    (category: CategoryDisplayModel, schedule: CategoryScheduleDisplayModel) => {
      setScheduleParent(category);
      setEditingSchedule(schedule);
      setScheduleModalOpen(true);
    },
    []
  );

  const closeScheduleModal = useCallback(() => {
    setScheduleModalOpen(false);
    setScheduleParent(null);
    setEditingSchedule(null);
  }, []);

  const handleDeleteSchedule = useCallback(
    (schedule: CategoryScheduleDisplayModel) => {
      modal.confirm({
        title: t('categorySchedules.confirmDelete'),
        content: t('categorySchedules.confirmDeleteMessage'),
        okText: t('categorySchedules.delete'),
        okType: 'danger',
        onOk: () => schedules.remove(schedule.categoryId, schedule.id),
      });
    },
    [schedules, modal, t]
  );

  const handleSaveSchedule = useCallback(
    async (data: CategoryScheduleFormData, id?: string, recurrence?: SchedulePayload) => {
      if (!scheduleParent) return;

      const payload = toSchedulePayload(data);
      if (id) {
        await schedules.update(scheduleParent.id, id, payload);
        notification.success({
          title: t('categories.success'),
          description: t('categorySchedules.scheduleUpdated'),
        });
      } else {
        await schedules.create(scheduleParent.id, payload, recurrence);
        notification.success({
          title: t('categories.success'),
          description: t('categorySchedules.scheduleAdded'),
        });
      }
      closeScheduleModal();
    },
    [schedules, scheduleParent, closeScheduleModal, notification, t]
  );

  const columns = getCategoryColumns(isMobile, t, openEditModal, handleDelete);

  return (
    <div>
      <div style={{ marginBottom: '1rem' }}>
        <Typography.Title level={isMobile ? 4 : 2} style={{ margin: 0 }}>
          {t('categories.allCategories')}
        </Typography.Title>
      </div>

      <div style={{ marginBottom: '1rem' }}>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={openAddModal}
          size={isMobile ? 'small' : 'middle'}
        >
          {t('categories.addCategory')}
        </Button>
      </div>

      {error && (
        <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
          {error}
        </Typography.Text>
      )}

      <Table
        columns={columns}
        dataSource={categories}
        loading={loading}
        rowKey="id"
        locale={{ emptyText: t('categories.noCategories') }}
        scroll={isMobile ? { x: 500 } : undefined}
        size={isMobile ? 'small' : 'middle'}
        expandable={{
          expandedRowRender: (category) => (
            <SchedulesPanel
              category={category}
              isMobile={isMobile}
              t={t}
              onAdd={openAddScheduleModal}
              onEdit={openEditScheduleModal}
              onDelete={handleDeleteSchedule}
            />
          ),
        }}
      />

      <CategoryFormModal
        open={modalOpen}
        onCancel={closeModal}
        onSave={handleSave}
        initialData={editingCategory}
      />

      <CategoryScheduleFormModal
        open={scheduleModalOpen}
        onCancel={closeScheduleModal}
        onSave={handleSaveSchedule}
        categoryName={scheduleParent?.name ?? ''}
        initialData={editingSchedule}
      />
    </div>
  );
};
