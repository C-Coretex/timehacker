import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import type { DynamicTaskReturnModel, FixedTaskFormData, InputDynamicTask } from '../../api/types';
import { PRIORITY_DEFAULT } from '../../utils/priority';
import { minutesToTimeSpan, timeSpanToMinutes } from '../../utils/timeUtils';
import type { TaskFormValues, TaskPrefill } from './types';

export const prefillValues = (prefill?: TaskPrefill): Partial<TaskFormValues> => ({
  priority: PRIORITY_DEFAULT,
  date: prefill?.date ?? dayjs(),
  startTime: prefill?.start,
  endTime: prefill?.end,
});

export const fixedTaskValues = (task: FixedTaskFormData): Partial<TaskFormValues> => ({
  name: task.name,
  description: task.description,
  categoryIds: task.categoryIds ?? [],
  priority: task.priority,
  date: dayjs(task.startTimestamp),
  startTime: dayjs(task.startTimestamp),
  endTime: dayjs(task.endTimestamp),
});

export const dynamicTaskValues = (task: DynamicTaskReturnModel): Partial<TaskFormValues> => ({
  name: task.name,
  description: task.description ?? '',
  categoryIds: task.categories.map((category) => category.id),
  priority: task.priority,
  minMinutes: timeSpanToMinutes(task.minTimeToFinish),
  maxMinutes: timeSpanToMinutes(task.maxTimeToFinish),
  optimalMinutes: task.optimalTimeToFinish ? timeSpanToMinutes(task.optimalTimeToFinish) : null,
});

// The day comes from the calendar card and the time of day from the time fields.
const atTimeOf = (day: Dayjs, time: Dayjs) => day.hour(time.hour()).minute(time.minute()).second(0).millisecond(0);

export const toFixedTaskFormData = (values: TaskFormValues): FixedTaskFormData => {
  const day = values.date ?? dayjs();
  return {
    name: values.name,
    description: values.description ?? '',
    categoryIds: values.categoryIds ?? [],
    priority: values.priority,
    startTimestamp: atTimeOf(day, values.startTime as Dayjs),
    endTimestamp: atTimeOf(day, values.endTime as Dayjs),
  };
};

export const toDynamicTaskInput = (values: TaskFormValues): InputDynamicTask => ({
  name: values.name,
  description: values.description || undefined,
  categoryIds: values.categoryIds ?? [],
  priority: values.priority,
  minTimeToFinish: minutesToTimeSpan(values.minMinutes ?? 0),
  maxTimeToFinish: minutesToTimeSpan(values.maxMinutes ?? 0),
  optimalTimeToFinish:
    values.optimalMinutes != null && values.optimalMinutes > 0 ? minutesToTimeSpan(values.optimalMinutes) : undefined,
});
