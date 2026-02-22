import { memo } from 'react';
import dayjs from 'dayjs';
import { useSettings } from '../../../contexts/SettingsContext';
import type { CalendarEvent } from '../../../utils/calendarUtils';

const CustomCalendarEvent = memo<{ event: CalendarEvent }>(({ event }) => {
  const { timeDisplayFormat } = useSettings();
  return (
    <div>
      <strong>{event.title}</strong>
      <div style={{ fontSize: '0.75em', opacity: 0.9 }}>
        {dayjs(event.start).format(timeDisplayFormat)} &rarr; {dayjs(event.end).format(timeDisplayFormat)}
      </div>
    </div>
  );
});

CustomCalendarEvent.displayName = 'CustomCalendarEvent';

export default CustomCalendarEvent;
