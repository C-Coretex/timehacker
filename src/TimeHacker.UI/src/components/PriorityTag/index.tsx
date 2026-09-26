import type { CSSProperties, FC } from 'react';
import { useTranslation } from 'react-i18next';
import { priorityColor, priorityLabel } from 'utils/priority';
import './styles.css';

/** A task priority as a tinted pill labelled Highest…Lowest. */
export const PriorityTag: FC<{ priority: number }> = ({ priority }) => {
  const { t } = useTranslation();

  return (
    <span className="th-priority-tag" style={{ '--th-priority-color': priorityColor(priority) } as CSSProperties}>
      {priorityLabel(priority, t)}
    </span>
  );
};
