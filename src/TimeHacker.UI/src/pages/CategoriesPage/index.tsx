import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useCategories, postNewScheduleForCategory } from '../../hooks/useCategories';
import { CategoryFormModal } from '../../components/CategoryFormModal';
import type { CategoryDisplayModel, CategoryFormData, InputCategory } from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';
import { useIsMobile } from '../../hooks/useIsMobile';
import { getCategoryColumns } from './columns';

const toPayload = (data: CategoryFormData): InputCategory => ({
  name: data.name,
  description: data.description || undefined,
  color: data.color,
  startTime: data.startTime.format('HH:mm:ss'),
  endTime: data.endTime.format('HH:mm:ss'),
});

export const CategoriesPage: FC = () => {
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { categories, loading, error, fetchCategories, create, update, remove } = useCategories();
  const { notification, modal } = App.useApp();

  const [modalOpen, setModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryDisplayModel | null>(null);

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
    async (data: CategoryFormData, id?: string, schedule?: SchedulePayload) => {
      try {
        const payload = toPayload(data);
        if (id) {
          await update(id, payload);
          notification.success({
            title: t('categories.success'),
            description: t('categories.categoryUpdated'),
          });
        } else {
          const newId = await create(payload);
          if (schedule && newId) {
            await postNewScheduleForCategory({
              parentEntityId: newId,
              repeatingEntityType: schedule.repeatingEntityType,
              endsOnModel: schedule.endsOnModel ?? undefined,
            });
          }
          notification.success({
            title: t('categories.success'),
            description: t('categories.categoryAdded'),
          });
          await fetchCategories();
        }
      } catch {
        notification.error({
          title: t('categories.error'),
          description: t('categories.categorySaveFailed'),
        });
      } finally {
        closeModal();
      }
    },
    [create, update, fetchCategories, closeModal, notification, t]
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
      />

      <CategoryFormModal
        open={modalOpen}
        onCancel={closeModal}
        onSave={handleSave}
        initialData={editingCategory}
      />
    </div>
  );
};
