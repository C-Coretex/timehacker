import dayjs from 'dayjs';
import type { FixedTaskFormData, InputFixedTask } from 'api/types';

/**
 * Builds the API payload for a fixed task from form data.
 * Timestamps are serialized as UTC ISO strings (`toISOString`) so the stored instant
 * is unambiguous and independent of the server's timezone.
 */
export function toFixedTaskPayload(data: FixedTaskFormData): InputFixedTask {
  return {
    name: data.name,
    description: data.description,
    categoryIds: data.categoryIds ?? [],
    priority: data.priority,
    startTimestamp: dayjs(data.startTimestamp).toISOString(),
    endTimestamp: dayjs(data.endTimestamp).toISOString(),
  };
}
