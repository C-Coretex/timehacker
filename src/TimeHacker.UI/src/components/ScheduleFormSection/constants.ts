import type { TFunction } from 'i18next';
import { RepeatingEntityTypeEnum } from 'api/types';
import type { PillOption } from 'components/PillGroup';

/** The "Does not repeat" pill; in the form it is an empty `scheduleType`. */
export const REPEAT_NONE = 'none';
export type FrequencyChoice = RepeatingEntityTypeEnum | typeof REPEAT_NONE;

export const getFrequencyOptions = (t: TFunction): PillOption<FrequencyChoice>[] => [
  { value: REPEAT_NONE, label: t('taskForm.doesNotRepeat') },
  { value: RepeatingEntityTypeEnum.DayRepeatingEntity, label: t('taskForm.repeatDaily') },
  { value: RepeatingEntityTypeEnum.WeekRepeatingEntity, label: t('taskForm.repeatWeekly') },
  { value: RepeatingEntityTypeEnum.MonthRepeatingEntity, label: t('taskForm.repeatMonthly') },
  { value: RepeatingEntityTypeEnum.YearRepeatingEntity, label: t('taskForm.repeatYearly') },
  { value: RepeatingEntityTypeEnum.OnceRepeatingEntity, label: t('taskForm.repeatOnDates') },
];

export type EndsMode = 'never' | 'date' | 'count';

export const getEndsOptions = (t: TFunction): PillOption<EndsMode>[] => [
  { value: 'never', label: t('taskForm.endsNever') },
  { value: 'date', label: t('taskForm.endsOnDate') },
  { value: 'count', label: t('taskForm.endsAfter') },
];
