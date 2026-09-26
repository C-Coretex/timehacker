import { useCallback, useState } from 'react';
import type { FC } from 'react';
import { App, Button, Table, Typography } from 'antd';
import { PlusCircleFilled } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { CategoryFormModal } from '../../components/CategoryFormModal';
import type { FirstCategoryWindow } from '../../components/CategoryFormModal/types';
import { CategoryScheduleFormModal } from '../../components/CategoryScheduleFormModal';
import { PageHeader } from '../../components/PageHeader';
import type { CategoryDisplayModel, CategoryFormData } from '../../api/types';
import { useCategories } from '../../hooks/useCategories';
import { useCategoryScheduleEditor } from '../../hooks/useCategoryScheduleEditor';
import { toCategoryPayload, toCategorySchedulePayload } from '../../utils/categoryPayloads';
import { CategoryCards } from './CategoryCards';
import { getCategoryColumns } from './columns';
import { SchedulesPanel } from './SchedulesPanel';

/** Categories as a table (cards on phones); each one opens onto the time windows that category occupies. */
export const CategoriesPage: FC = () => {
  const { t } = useTranslation();
  const { notification, modal } = App.useApp();
  const { categories, loading, error, fetchCategories, create, update, remove } = useCategories();
  const windows = useCategoryScheduleEditor(fetchCategories);

  const [modalOpen, setModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<CategoryDisplayModel | null>(null);

  const openModal = useCallback((category: CategoryDisplayModel | null) => {
    setEditingCategory(category);
    setModalOpen(true);
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
    async (data: CategoryFormData, id?: string, firstWindow?: FirstCategoryWindow) => {
      const payload = toCategoryPayload(data);
      if (id) {
        if (!(await update(id, payload))) return;
        notification.success({ title: t('categories.success'), description: t('categories.categoryUpdated') });
        setModalOpen(false);
        return;
      }

      const newId = await create(payload);
      if (!newId) return;
      setModalOpen(false);
      // Two calls, not one transaction: if the window fails the category still exists and the window can be
      // added from its row, which the warning says.
      const windowAdded =
        !firstWindow ||
        (await windows.createWindow(newId, toCategorySchedulePayload(firstWindow.schedule), firstWindow.recurrence));
      if (windowAdded) {
        notification.success({ title: t('categories.success'), description: t('categories.categoryAdded') });
      } else {
        notification.warning({ title: t('categories.categoryAdded'), description: t('categoryForm.windowFailed') });
      }
    },
    [create, update, windows, notification, t]
  );

  return (
    <div>
      <PageHeader
        title={t('categories.allCategories')}
        actions={
          <Button type="primary" icon={<PlusCircleFilled />} iconPlacement="end" onClick={() => openModal(null)}>
            {t('categories.addCategory')}
          </Button>
        }
      />

      {error && (
        <Typography.Text type="danger" style={{ display: 'block', marginBottom: 8 }}>
          {error}
        </Typography.Text>
      )}

      <Table
        className="th-desktop-only"
        columns={getCategoryColumns(t, openModal, handleDelete)}
        dataSource={categories}
        loading={loading}
        rowKey="id"
        locale={{ emptyText: t('categories.noCategories') }}
        scroll={{ x: 'max-content' }}
        size="middle"
        expandable={{
          expandedRowRender: (category) => (
            <SchedulesPanel
              category={category}
              t={t}
              onAdd={windows.openAdd}
              onEdit={windows.openEdit}
              onDelete={windows.remove}
            />
          ),
        }}
      />
      <CategoryCards
        className="th-phone-only"
        categories={categories}
        loading={loading}
        onEdit={openModal}
        onDelete={handleDelete}
        onAddWindow={windows.openAdd}
        onEditWindow={windows.openEdit}
        onDeleteWindow={windows.remove}
      />

      <CategoryFormModal
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onSave={handleSave}
        initialData={editingCategory}
      />

      <CategoryScheduleFormModal
        open={windows.isOpen}
        onCancel={windows.close}
        onSave={windows.save}
        categoryName={windows.category?.name ?? ''}
        initialData={windows.editing}
      />
    </div>
  );
};
