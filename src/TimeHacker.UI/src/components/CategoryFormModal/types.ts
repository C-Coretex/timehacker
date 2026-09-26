import type { CategoryDisplayModel, CategoryFormData, CategoryScheduleFormData } from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';

/** The optional time window created together with a new category. */
export interface FirstCategoryWindow {
  schedule: CategoryScheduleFormData;
  recurrence?: SchedulePayload;
}

export interface CategoryFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSave: (data: CategoryFormData, id?: string, firstWindow?: FirstCategoryWindow) => void;
  initialData?: CategoryDisplayModel | null;
}
