import type { CategoryFormData, CategoryScheduleFormData, InputCategory, InputCategorySchedule } from '../api/types';

export const toCategoryPayload = (data: CategoryFormData): InputCategory => ({
  name: data.name,
  description: data.description || undefined,
  color: data.color,
});

/** Category windows are wall-clock `TimeOnly` values, so the times go out without any UTC conversion. */
export const toCategorySchedulePayload = (data: CategoryScheduleFormData): InputCategorySchedule => ({
  description: data.description || undefined,
  date: data.date.format('YYYY-MM-DD'),
  startTime: data.startTime.format('HH:mm:ss'),
  endTime: data.endTime.format('HH:mm:ss'),
});
