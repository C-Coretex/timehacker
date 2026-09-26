import type { FC } from 'react';
import { useNavigate } from 'react-router-dom';
import dayjs from 'dayjs';
import { CalendarCard } from 'components/CalendarCard';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import { visibleRange } from 'utils/plannerRange';

/** Sidebar month card that drives the planner: it tints the visible range and jumps the planner to a picked day. */
export const MiniCalendar: FC = () => {
  const navigate = useNavigate();
  const { selectedDate, setSelectedDate, calendarView } = useCalendarDate();
  const { weekStartDay } = useSettings();
  // Day and month views need no tint: the selection, or the whole grid, already says what is shown.
  const showsRange = calendarView === 'week' || calendarView === '3day';

  return (
    <CalendarCard
      className="th-mini-calendar"
      value={dayjs(selectedDate)}
      highlight={showsRange ? visibleRange(calendarView, selectedDate, weekStartDay) : undefined}
      onChange={(date) => {
        setSelectedDate(date.toDate());
        navigate('/');
      }}
    />
  );
};
