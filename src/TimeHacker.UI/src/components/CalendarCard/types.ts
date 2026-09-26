import type { ReactNode } from 'react';
import type { Dayjs } from 'dayjs';

export interface CalendarCardProps {
  /** The selected day. Works as a Form.Item child: `value` + `onChange`. */
  value?: Dayjs | null;
  onChange?: (date: Dayjs) => void;
  /** Days tinted as a range, e.g. the week the planner is showing. */
  highlight?: { start: Dayjs; end: Dayjs };
  /** Rendered under the grid, inside the card (repeat picker, time fields). */
  footer?: ReactNode;
  /** Phones fold the month grid into a date row that opens it (forms); picking a day folds it again. */
  collapsible?: boolean;
  className?: string;
}
