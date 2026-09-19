import { memo } from 'react';
import dayjs from 'dayjs';
import { useSettings } from '../../../contexts/SettingsContext';
import { CategoryDots } from '../../../components/CategoryDots';
import type { CalendarEvent } from '../../../utils/calendarUtils';

export const CustomCalendarEvent = memo<{ event: CalendarEvent }>(({ event }) => {
  const { timeDisplayFormat } = useSettings();

  // A category band spans the whole window with tasks drawn over it, so its name is rendered as a
  // compact pill pinned to the top-left rail rather than as a full event body — that pill is what stays
  // readable on a day packed with tasks.
  if (event.resource?.type === 'category') {
    return (
      <div className="th-category-label">
        {event.title}
        <span className="th-category-label-time">
          {dayjs(event.start).format(timeDisplayFormat)}&nbsp;&rarr;&nbsp;
          {dayjs(event.end).format(timeDisplayFormat)}
        </span>
      </div>
    );
  }

  // Dots rather than labels: an event body is often only a couple of lines tall, and each dot names its
  // category on hover.
  const categories = event.resource?.categories ?? [];

  return (
    <div>
      <strong>{event.title}</strong>
      <div style={{ display: 'flex', alignItems: 'center', gap: 6, fontSize: '0.75em', opacity: 0.9 }}>
        <span>
          {dayjs(event.start).format(timeDisplayFormat)} &rarr; {dayjs(event.end).format(timeDisplayFormat)}
        </span>
        <CategoryDots categories={categories} />
      </div>
    </div>
  );
});

CustomCalendarEvent.displayName = 'CustomCalendarEvent';
