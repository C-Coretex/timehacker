import type { FC } from 'react';
import { SyncOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { ListCard } from 'components/ListCard';
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
    <ListCard
      title={event.title}
      categories={resource.categories}
      description={event.description}
      tone={resource.type}
      accent={firstCategory ? argbToHex(firstCategory.color) : undefined}
      className={['th-today-row', emphasis && `th-today-row--${emphasis}`, isPast && 'is-past'].filter(Boolean).join(' ')}
      onOpen={() => onOpen(event)}
      meta={
        <>
          <span>{resource.isFixed ? t('calendar.fixed') : t('calendar.dynamic')}</span>
          {resource.categories.length > 0 && <strong>{resource.categories.map((category) => category.name).join(', ')}</strong>}
          <PriorityTag priority={resource.task.priority} />
        </>
      }
      aside={
        <>
          {resource.scheduleEntityId && <SyncOutlined aria-label={t('calendar.recurring')} />}
          {dayjs(event.start).format(timeDisplayFormat)}
        </>
      }
    />
  );
};
