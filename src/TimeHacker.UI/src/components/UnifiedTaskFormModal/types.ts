import type { ScheduleEntityReturnModel, FixedTaskFormData, InputDynamicTask, DynamicTaskReturnModel } from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';

export type TaskTab = 'fixed' | 'dynamic';

/** Kept as the task modal's public name for the shared schedule payload. */
export type ScheduleFormPayload = SchedulePayload;

export interface UnifiedTaskFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSaveFixed: (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => void;
  onSaveDynamic: (data: InputDynamicTask, id?: string) => void;
  initialFixedData?: FixedTaskFormData & { id: string; scheduleEntity?: ScheduleEntityReturnModel | null };
  initialDynamicData?: DynamicTaskReturnModel | null;
  initialTab?: TaskTab;
  defaultDate?: Date;
}
