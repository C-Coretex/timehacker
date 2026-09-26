import type { Dayjs } from 'dayjs';
import type {
  DynamicTaskReturnModel,
  FixedTaskFormData,
  InputDynamicTask,
  ScheduleEntityReturnModel,
} from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';

export type TaskTab = 'fixed' | 'dynamic';

/** Kept as the task modal's public name for the shared schedule payload. */
export type ScheduleFormPayload = SchedulePayload;

/** Where a new task starts: the day, and optionally the slot picked on the planner. */
export interface TaskPrefill {
  date: Dayjs;
  start?: Dayjs;
  end?: Dayjs;
}

export interface UnifiedTaskFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSaveFixed: (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => void;
  onSaveDynamic: (data: InputDynamicTask, id?: string) => void;
  initialFixedData?: FixedTaskFormData & { id: string; scheduleEntity?: ScheduleEntityReturnModel | null };
  initialDynamicData?: DynamicTaskReturnModel | null;
  initialTab?: TaskTab;
  prefill?: TaskPrefill;
}

/** Everything the task form holds; the recurrence fields ride along untyped for `buildSchedulePayload`. */
export interface TaskFormValues extends Record<string, unknown> {
  name: string;
  description?: string;
  categoryIds?: string[];
  priority: number;
  date?: Dayjs;
  startTime?: Dayjs;
  endTime?: Dayjs;
  minMinutes?: number;
  maxMinutes?: number;
  optimalMinutes?: number | null;
}
