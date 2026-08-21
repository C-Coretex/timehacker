import dayjs from 'dayjs';
import type { TFunction } from 'i18next';
import { RepeatingEntityTypeEnum } from '../api/types';
import type { ReturnRepeatingEntityModel } from '../api/types';

const DAY_KEYS = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'] as const;

/** Human-readable summary of a recurrence, shared by the calendar popup and the task/category modals. */
export function describeRecurrence(repeatingEntity: ReturnRepeatingEntityModel, t: TFunction): string {
  switch (repeatingEntity.entityType) {
    case RepeatingEntityTypeEnum.OnceRepeatingEntity:
      return t('taskForm.repeatsOnDates', {
        dates: [...repeatingEntity.dates]
          .sort()
          .map((d) => dayjs(d).format('MMM D, YYYY'))
          .join(', '),
      });
    case RepeatingEntityTypeEnum.DayRepeatingEntity:
      return t('taskForm.repeatsEveryNDays', { count: repeatingEntity.daysCountToRepeat });
    case RepeatingEntityTypeEnum.WeekRepeatingEntity:
      return t('taskForm.repeatsWeeklyOn', {
        days: [...repeatingEntity.repeatsOn]
          .sort()
          .map((d) => t(`taskForm.${DAY_KEYS[d - 1]}`))
          .join(', '),
      });
    case RepeatingEntityTypeEnum.MonthRepeatingEntity:
      return t('taskForm.repeatsMonthlyOnDay', { day: repeatingEntity.monthDayToRepeat });
    case RepeatingEntityTypeEnum.YearRepeatingEntity:
      return t('taskForm.repeatsYearlyOnDay', { day: repeatingEntity.yearDayToRepeat });
  }
}

/** Short badge label for a recurrence type, used in table columns. */
export function recurrenceTypeLabel(entityType: RepeatingEntityTypeEnum, t: TFunction): string {
  switch (entityType) {
    case RepeatingEntityTypeEnum.OnceRepeatingEntity:
      return t('tasks.specificDates');
    case RepeatingEntityTypeEnum.DayRepeatingEntity:
      return t('tasks.daily');
    case RepeatingEntityTypeEnum.WeekRepeatingEntity:
      return t('tasks.weekly');
    case RepeatingEntityTypeEnum.MonthRepeatingEntity:
      return t('tasks.monthly');
    case RepeatingEntityTypeEnum.YearRepeatingEntity:
      return t('tasks.yearly');
  }
}
