import type { EndsOnModel, InputRepeatingEntityType, ScheduleEntityReturnModel, FixedTaskFormData, InputDynamicTask, DynamicTaskReturnModel } from '../../api/types';

export type TaskTab = 'fixed' | 'dynamic';

export type ScheduleFormPayload = {
  repeatingEntityType: InputRepeatingEntityType;
  endsOnModel?: EndsOnModel | null;
};

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
