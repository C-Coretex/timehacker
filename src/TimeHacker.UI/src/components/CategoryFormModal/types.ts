import type { CategoryDisplayModel, CategoryFormData } from '../../api/types';
import type { SchedulePayload } from '../../utils/buildSchedulePayload';

export interface CategoryFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSave: (data: CategoryFormData, id?: string, schedule?: SchedulePayload) => void;
  initialData?: CategoryDisplayModel | null;
  /** Pre-selects this date in the "specific dates" schedule field when creating from the calendar. */
  defaultDate?: Date;
}
