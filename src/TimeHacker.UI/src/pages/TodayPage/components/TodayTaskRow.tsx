import type { CSSProperties, FC } from 'react';
import { SyncOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { CategoryDots } from 'components/CategoryDots';
import { PriorityTag } from 'components/PriorityTag';
import { useSettings } from 'contexts/SettingsContext';
import type { TaskCalendarEvent } from 'utils/calendarUtils';
import { argbToHex } from 'utils/colorArgb';
import { priorityEmphasis } from 'utils/priority';

interface TodayTaskRowProps {
  event: TaskCalendarEvent;
  isPast: boolean;
  onOpen: (event: TaskCalendarEvent) => void;
}

/** One of today's tasks as the design's list row: type colour, title with category dots, ⟳ when recurring, time on the right. */
export const TodayTaskRow: FC<TodayTaskRowProps> = ({ event, isPast, onOpen }) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();
  const { resource } = event;
  const [firstCategory] = resource.categories;
  const emphasis = priorityEmphasis(resource.task.priority);

  return (
    <button
      type="button"
      className={[
        'th-today-row',
        `th-today-row--${resource.type}`,
        emphasis && `th-today-row--${emphasis}`,
        isPast && 'is-past',
      ]
        .filter(Boolean)
        .join(' ')}
      style={firstCategory ? ({ '--th-today-accent': argbToHex(firstCategory.color) } as CSSProperties) : undefined}
      onClick={() => onOpen(event)}
    >
      <span className="th-today-row__main">
        <span className="th-today-row__heading">
          <span className="th-today-row__title">{event.title}</span>
          <CategoryDots categories={resource.categories} />
        </span>
        {event.description && <span className="th-today-row__description">{event.description}</span>}
        <span className="th-today-row__meta">
          <span>{resource.isFixed ? t('calendar.fixed') : t('calendar.dynamic')}</span>
          {resource.categories.length > 0 && (
            <span className="th-today-row__category">{resource.categories.map((category) => category.name).join(', ')}</span>
          )}
          <PriorityTag priority={resource.task.priority} />
        </span>
      </span>
      <span className="th-today-row__time">
        {resource.scheduleEntityId && <SyncOutlined aria-label={t('calendar.recurring')} />}
        {dayjs(event.start).format(timeDisplayFormat)}
      </span>
    </button>
  );
};
