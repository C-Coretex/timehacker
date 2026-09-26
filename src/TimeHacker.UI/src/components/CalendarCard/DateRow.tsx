import type { FC } from 'react';
import { CalendarOutlined, DownOutlined, UpOutlined } from '@ant-design/icons';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

interface DateRowProps {
  value?: Dayjs | null;
  expanded: boolean;
  onToggle: () => void;
}

/** A collapsible card's folded state on phones: the chosen day, opening the month grid when tapped. */
export const CalendarCardDateRow: FC<DateRowProps> = ({ value, expanded, onToggle }) => {
  const { t } = useTranslation();

  return (
    <button type="button" className="th-calendar-card__date-row th-phone-only" aria-expanded={expanded} onClick={onToggle}>
      <CalendarOutlined className="th-calendar-card__date-icon" />
      <span className="th-calendar-card__date-text">{value ? value.format('dddd, D MMMM YYYY') : t('calendar.pickDay')}</span>
      {expanded ? <UpOutlined className="th-calendar-card__date-chevron" /> : <DownOutlined className="th-calendar-card__date-chevron" />}
    </button>
  );
};
