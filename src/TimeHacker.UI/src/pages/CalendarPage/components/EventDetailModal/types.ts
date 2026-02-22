import type { CalendarEvent } from '../../../../utils/calendarUtils';
import type { ScheduleEntityReturnModel } from '../../../../api/types';

export interface EventDetailModalProps {
  open: boolean;
  onClose: () => void;
  event: CalendarEvent | null;
  scheduleData: ScheduleEntityReturnModel | null;
  loadingSchedule: boolean;
  timeDisplayFormat: string;
}
