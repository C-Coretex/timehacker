import type { FC } from 'react';
import dayjs from 'dayjs';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import './styles.css';

/**
 * Time-grid column header: "Mon (15) Aug", today's number filled and the selected day underlined. rbc wraps
 * it in its own drill-down button, so nothing in here may be interactive.
 */
export const DayHeader: FC<{ date: Date }> = ({ date }) => {
  const { selectedDate } = useCalendarDate();
  const day = dayjs(date);

  return (
    <span className={`th-day-header${day.isSame(selectedDate, 'day') ? ' is-selected' : ''}`}>
      <span className="th-day-header__weekday">{day.format('ddd')}</span>
      <span className={`th-day-header__number${day.isSame(dayjs(), 'day') ? ' is-today' : ''}`}>{day.date()}</span>
      <span className="th-day-header__month">{day.format('MMM')}</span>
    </span>
  );
};

/** Month-view weekday header. Passed explicitly: rbc would otherwise fall back to DayHeader there. */
export const WeekdayHeader: FC<{ date: Date }> = ({ date }) => (
  <span className="th-weekday-header">{dayjs(date).format('ddd')}</span>
);
