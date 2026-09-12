import type { CategoryScheduleDisplayModel, CategoryScheduleFormData } from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';

export interface CategoryScheduleFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSave: (data: CategoryScheduleFormData, id?: string, recurrence?: SchedulePayload) => void;
  /** The category this window belongs to — shown in the title so the context is never ambiguous. */
  categoryName: string;
  initialData?: CategoryScheduleDisplayModel | null;
  /** Pre-selects this date when adding a window from the calendar. */
  defaultDate?: Date;
}
