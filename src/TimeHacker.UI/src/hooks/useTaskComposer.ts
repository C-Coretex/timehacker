import { useCallback, useState } from 'react';
import { App } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { createDynamicTask } from 'api/dynamicTasks';
import { createFixedTaskWithSchedule } from 'api/fixedTasks';
import type { FixedTaskFormData, InputDynamicTask } from 'api/types';
import type { TaskPrefill } from 'components/UnifiedTaskFormModal';
import type { SchedulePayload } from 'utils/buildSchedulePayload';
import { toFixedTaskPayload } from 'utils/fixedTaskPayload';

/**
 * The "add task" dialog of a page that shows the plan (planner, today): where it opens pre-filled, and the
 * create calls for both task types. `onCreated` reloads whatever the page is showing.
 */
export function useTaskComposer(onCreated: () => Promise<unknown>) {
  const { notification } = App.useApp();
  const { t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);
  const [prefill, setPrefill] = useState<TaskPrefill>(() => ({ date: dayjs() }));

  const open = useCallback((next?: Partial<TaskPrefill>) => {
    setPrefill({ date: dayjs(), ...next });
    setIsOpen(true);
  }, []);

  const close = useCallback(() => setIsOpen(false), []);

  const saveFixed = useCallback(
    async (data: FixedTaskFormData, _id?: string, schedule?: SchedulePayload) => {
      try {
        await createFixedTaskWithSchedule(toFixedTaskPayload(data), schedule);
        setIsOpen(false);
        notification.success({ title: t('tasks.success'), description: t('tasks.fixedTaskAdded') });
        await onCreated();
      } catch {
        notification.error({ title: t('tasks.error'), description: t('tasks.fixedTaskSaveFailed') });
      }
    },
    [notification, onCreated, t]
  );

  const saveDynamic = useCallback(
    async (data: InputDynamicTask) => {
      try {
        await createDynamicTask(data);
        setIsOpen(false);
        notification.success({ title: t('tasks.success'), description: t('tasks.dynamicTaskAdded') });
        await onCreated();
      } catch {
        notification.error({ title: t('tasks.error'), description: t('tasks.dynamicTaskSaveFailed') });
      }
    },
    [notification, onCreated, t]
  );

  return { isOpen, prefill, open, close, saveFixed, saveDynamic };
}
