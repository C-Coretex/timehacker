import { useCallback } from 'react';
import { App } from 'antd';
import { useTranslation } from 'react-i18next';
import {
  createCategorySchedule,
  deleteCategorySchedule,
  postNewRecurrenceForCategorySchedule,
  updateCategorySchedule,
} from '../api/categorySchedules';
import type { InputCategorySchedule } from '../api/types';
import type { SchedulePayload } from '../utils/buildSchedulePayload';

interface UseCategorySchedulesOptions {
  /** Runs after every successful mutation — schedules arrive nested in the category list. */
  onChanged: () => Promise<void>;
}

/**
 * Mutations for a category's time windows. There is no list to own here: the windows come back on the
 * category itself, so the caller refetches categories rather than this hook holding its own state.
 */
export const useCategorySchedules = ({ onChanged }: UseCategorySchedulesOptions) => {
  const { notification } = App.useApp();
  const { t } = useTranslation();

  const run = useCallback(
    async (action: () => Promise<unknown>, errorMessage: string) => {
      try {
        await action();
        await onChanged();
      } catch {
        notification.error({ title: t('errors.generic'), description: errorMessage });
      }
    },
    [notification, onChanged, t]
  );

  // The recurrence is a second call: the window must exist before anything can repeat it.
  const create = useCallback(
    async (categoryId: string, schedule: InputCategorySchedule, recurrence?: SchedulePayload) =>
      run(async () => {
        const id = await createCategorySchedule(categoryId, schedule);
        if (recurrence) {
          await postNewRecurrenceForCategorySchedule({
            parentEntityId: id,
            repeatingEntityType: recurrence.repeatingEntityType,
            endsOnModel: recurrence.endsOnModel ?? undefined,
          });
        }
      }, t('categorySchedules.saveFailed')),
    [run, t]
  );

  const update = useCallback(
    async (categoryId: string, id: string, schedule: InputCategorySchedule) =>
      run(() => updateCategorySchedule(categoryId, id, schedule), t('categorySchedules.saveFailed')),
    [run, t]
  );

  const remove = useCallback(
    async (categoryId: string, id: string) =>
      run(() => deleteCategorySchedule(categoryId, id), t('categorySchedules.deleteFailed')),
    [run, t]
  );

  return { create, update, remove };
};
