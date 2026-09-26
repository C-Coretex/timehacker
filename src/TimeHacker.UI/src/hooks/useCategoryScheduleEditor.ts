import { useCallback, useState } from 'react';
import { App } from 'antd';
import { useTranslation } from 'react-i18next';
import type { CategoryDisplayModel, CategoryScheduleDisplayModel, CategoryScheduleFormData } from '../api/types';
import type { SchedulePayload } from '../utils/buildSchedulePayload';
import { toCategorySchedulePayload } from '../utils/categoryPayloads';
import { useCategorySchedules } from './useCategorySchedules';

/**
 * State and actions behind the categories page's window dialog: which category and window are being
 * edited, and the create/update/delete calls with their confirmations and messages.
 */
export function useCategoryScheduleEditor(onChanged: () => Promise<void>) {
  const { t } = useTranslation();
  const { notification, modal } = App.useApp();
  const schedules = useCategorySchedules({ onChanged });

  const [isOpen, setIsOpen] = useState(false);
  const [category, setCategory] = useState<CategoryDisplayModel | null>(null);
  const [editing, setEditing] = useState<CategoryScheduleDisplayModel | null>(null);

  const openAdd = useCallback((parent: CategoryDisplayModel) => {
    setCategory(parent);
    setEditing(null);
    setIsOpen(true);
  }, []);

  const openEdit = useCallback((parent: CategoryDisplayModel, schedule: CategoryScheduleDisplayModel) => {
    setCategory(parent);
    setEditing(schedule);
    setIsOpen(true);
  }, []);

  const close = useCallback(() => setIsOpen(false), []);

  const remove = useCallback(
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

  const save = useCallback(
    async (data: CategoryScheduleFormData, id?: string, recurrence?: SchedulePayload) => {
      if (!category) return;
      const payload = toCategorySchedulePayload(data);
      const saved = id
        ? await schedules.update(category.id, id, payload)
        : await schedules.create(category.id, payload, recurrence);
      if (!saved) return;

      notification.success({
        title: t('categories.success'),
        description: id ? t('categorySchedules.scheduleUpdated') : t('categorySchedules.scheduleAdded'),
      });
      setIsOpen(false);
    },
    [category, schedules, notification, t]
  );

  return { isOpen, category, editing, openAdd, openEdit, close, remove, save, createWindow: schedules.create };
}
