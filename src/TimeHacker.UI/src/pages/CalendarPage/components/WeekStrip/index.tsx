import type { FC } from 'react';
import dayjs from 'dayjs';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import { daysIn, visibleRange } from 'utils/plannerRange';
import './styles.css';

/** Phone day view's week picker: weekday initials over circled dates, the shown day filled. */
export const WeekStrip: FC = () => {
  const { selectedDate, setSelectedDate } = useCalendarDate();
  const { weekStartDay } = useSettings();
  const week = daysIn(visibleRange('week', selectedDate, weekStartDay));

  return (
    <div className="th-week-strip" role="group">
      {week.map((date) => {
        const day = dayjs(date);
        const isSelected = day.isSame(selectedDate, 'day');
        return (
          <button
            key={date.toISOString()}
            type="button"
            className={['th-week-strip__day', isSelected && 'is-selected', day.isSame(dayjs(), 'day') && 'is-today']
              .filter(Boolean)
              .join(' ')}
            aria-pressed={isSelected}
            aria-label={day.format('dddd, D MMMM')}
            onClick={() => setSelectedDate(date)}
          >
            <span className="th-week-strip__weekday">{day.format('dd').charAt(0)}</span>
            <span className="th-week-strip__number">{day.date()}</span>
          </button>
        );
      })}
    </div>
  );
};
