import type { FC } from 'react';
import { ClockCircleOutlined, SyncOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { CategoryDots } from 'components/CategoryDots';
import { useSettings } from 'contexts/SettingsContext';
import { isTaskEvent } from 'utils/calendarUtils';
import type { CalendarEvent } from 'utils/calendarUtils';

/**
 * Every part of a task is rendered; `styles.css` container queries decide which survive at the event's
 * actual size (card → two lines → one line → "…"), since rbc only knows that size after layout. Whatever is
 * hidden stays in the hover tooltip.
 */
export const TaskEventCard: FC<{ event: CalendarEvent }> = ({ event }) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();
  const resource = isTaskEvent(event) ? event.resource : undefined;
  const time = `${dayjs(event.start).format(timeDisplayFormat)} → ${dayjs(event.end).format(timeDisplayFormat)}`;
  const tooltip = [`${event.title} · ${time}`, event.description].filter(Boolean).join('\n');

  return (
    <div className="th-event" title={tooltip}>
      <div className="th-event__heading">
        <span className="th-event__title">{event.title}</span>
        <CategoryDots categories={resource?.categories ?? []} />
        {resource?.scheduleEntityId && <SyncOutlined className="th-event__repeat" aria-label={t('calendar.recurring')} />}
        <span className="th-event__inline-time">{time}</span>
      </div>
      {event.description && <div className="th-event__description">{event.description}</div>}
      <div className="th-event__time">
        <ClockCircleOutlined />
        <span className="th-event__time-text">{time}</span>
      </div>
      <span className="th-event__more" aria-hidden>
        …
      </span>
    </div>
  );
};
