import type { ReactNode } from 'react';
import type { CategoryReturnModel } from 'api/types';

export interface ListCardProps {
  title: ReactNode;
  /** Shown as colour dots beside the title. */
  categories?: CategoryReturnModel[];
  description?: string | null;
  /** The small line under the title: type, category names (in `<strong>`), priority, recurrence. */
  meta?: ReactNode;
  /** Right-aligned summary — a time, a duration, a count. */
  aside?: ReactNode;
  /** The left rail's colour, any CSS colour; transparent when unset. */
  accent?: string;
  /** The fill: the planner's fixed or dynamic row tint, or a plain bordered card. */
  tone?: 'fixed' | 'dynamic' | 'plain';
  /** Tapping the row — opens or edits the item. */
  onOpen: () => void;
  /** Adds a trailing delete button, named for screen readers by `deleteLabel`. */
  onDelete?: () => void;
  deleteLabel?: string;
  /** State and emphasis classes from the caller, e.g. `is-past`. */
  className?: string;
  /** Content under the row and outside its tap target, e.g. a category's windows. */
  footer?: ReactNode;
}
