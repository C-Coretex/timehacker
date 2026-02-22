import type { TFunction } from 'i18next';
import { RepeatingEntityTypeEnum } from '../../api/types';

export const getRepeatTypes = (t: TFunction) => [
  { value: RepeatingEntityTypeEnum.DayRepeatingEntity, label: t('taskForm.everyNDays') },
  { value: RepeatingEntityTypeEnum.WeekRepeatingEntity, label: t('taskForm.weekly') },
  { value: RepeatingEntityTypeEnum.MonthRepeatingEntity, label: t('taskForm.monthly') },
  { value: RepeatingEntityTypeEnum.YearRepeatingEntity, label: t('taskForm.yearly') },
];

export const getDaysOfWeek = (t: TFunction) => [
  { value: 1, label: t('taskForm.mon') },
  { value: 2, label: t('taskForm.tue') },
  { value: 3, label: t('taskForm.wed') },
  { value: 4, label: t('taskForm.thu') },
  { value: 5, label: t('taskForm.fri') },
  { value: 6, label: t('taskForm.sat') },
  { value: 7, label: t('taskForm.sun') },
];

export const getFixedPriorityMarks = (t: TFunction) => ({
  1: t('taskForm.priorityLow'),
  5: t('taskForm.priorityNormal'),
  10: t('taskForm.priorityHigh'),
});

export const getDynamicPriorityMarks = (t: TFunction) => ({
  0: t('taskForm.priorityLow'),
  128: t('taskForm.priorityNormal'),
  255: t('taskForm.priorityHigh'),
});
