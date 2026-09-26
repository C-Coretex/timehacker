import type { FC } from 'react';
import type { Dayjs } from 'dayjs';
import { DoubleLeftOutlined, DoubleRightOutlined, LeftOutlined, RightOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

interface HeaderProps {
  panel: Dayjs;
  onNavigate: (panel: Dayjs) => void;
}

export const CalendarCardHeader: FC<HeaderProps> = ({ panel, onNavigate }) => {
  const { t } = useTranslation();

  return (
    <div className="th-calendar-card__header">
      <button type="button" aria-label={t('calendar.previousYear')} onClick={() => onNavigate(panel.subtract(1, 'year'))}>
        <DoubleLeftOutlined />
      </button>
      <button type="button" aria-label={t('calendar.previousMonth')} onClick={() => onNavigate(panel.subtract(1, 'month'))}>
        <LeftOutlined />
      </button>
      <span className="th-calendar-card__title">{panel.format('MMMM YYYY')}</span>
      <button type="button" aria-label={t('calendar.nextMonth')} onClick={() => onNavigate(panel.add(1, 'month'))}>
        <RightOutlined />
      </button>
      <button type="button" aria-label={t('calendar.nextYear')} onClick={() => onNavigate(panel.add(1, 'year'))}>
        <DoubleRightOutlined />
      </button>
    </div>
  );
};
