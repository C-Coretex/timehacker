import { useMemo } from 'react';
import type { FC } from 'react';
import { Calendar, dayjsLocalizer } from 'react-big-calendar';
import type { SlotInfo, View } from 'react-big-calendar';
import dayjs from 'dayjs';
import { Spin } from 'antd';
import { useTranslation } from 'react-i18next';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import type { CalendarView } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import type { CalendarEvent } from 'utils/calendarUtils';
import { DayHeader, WeekdayHeader } from './components/DayHeader';
import { MonthDateHeader } from './components/MonthDateHeader';
import { PlannerEvent } from './components/PlannerEvent';
import { QuickAddSlot } from './components/QuickAddSlot';
import { eventPropGetter } from './eventPropGetter';
import { MonthEventsContext, monthDayKey } from './monthEventsContext';
import { QuickAddContext } from './quickAddContext';
import { ThreeDayView } from './ThreeDayView';
import './calendar-theme.css';

const VIEWS = { month: true, week: true, day: true, '3day': ThreeDayView };

// Module scope: rbc rebuilds its view tree when the components object changes identity.
const COMPONENTS = {
  event: PlannerEvent,
  header: DayHeader,
  month: { header: WeekdayHeader, dateHeader: MonthDateHeader },
  timeSlotWrapper: QuickAddSlot,
};

// Opens the time grid at the start of a working morning rather than at midnight.
const SCROLL_TO = new Date(1970, 0, 1, 7, 30);

interface PlannerCalendarProps {
  events: CalendarEvent[];
  backgroundEvents: CalendarEvent[];
  loading: boolean;
  onSelectEvent: (event: CalendarEvent) => void;
  onSelectSlot: (slot: SlotInfo) => void;
  /** The hour's "+" button was clicked. */
  onQuickAdd: (hourStart: Date) => void;
}

/** react-big-calendar configured for the planner. Date and view are controlled by CalendarDateContext. */
export const PlannerCalendar: FC<PlannerCalendarProps> = ({
  events,
  backgroundEvents,
  loading,
  onSelectEvent,
  onSelectSlot,
  onQuickAdd,
}) => {
  const { i18n } = useTranslation();
  const { timeFormat, timeDisplayFormat, weekStartDay } = useSettings();
  const { selectedDate, setSelectedDate, calendarView, setCalendarView } = useCalendarDate();

  // The week start lives on the dayjs locale (see applyWeekStart); a fresh localizer makes rbc re-read it.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  const localizer = useMemo(() => dayjsLocalizer(dayjs), [weekStartDay]);

  const formats = useMemo(
    () => ({
      timeGutterFormat: timeFormat === '12h' ? 'h A' : 'HH:mm',
      eventTimeRangeFormat: () => '',
      selectRangeFormat: ({ start, end }: { start: Date; end: Date }) =>
        `${dayjs(start).format(timeDisplayFormat)} – ${dayjs(end).format(timeDisplayFormat)}`,
    }),
    [timeFormat, timeDisplayFormat]
  );

  const eventsByDay = useMemo(
    () =>
      events.reduce((byDay, event) => {
        const key = monthDayKey(event.start);
        byDay.set(key, [...(byDay.get(key) ?? []), event]);
        return byDay;
      }, new Map<string, CalendarEvent[]>()),
    [events]
  );

  return (
    <div className="th-planner-frame">
      <QuickAddContext.Provider value={onQuickAdd}>
        <MonthEventsContext.Provider value={eventsByDay}>
          <Calendar
            className="th-planner"
            localizer={localizer}
            culture={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
            events={events}
            backgroundEvents={backgroundEvents}
            views={VIEWS}
            view={calendarView as View}
            onView={(view) => setCalendarView(view as CalendarView)}
            date={selectedDate}
            onNavigate={setSelectedDate}
            onDrillDown={(date) => {
              setSelectedDate(date);
              setCalendarView('day');
            }}
            toolbar={false}
            selectable="ignoreEvents"
            onSelectSlot={onSelectSlot}
            onSelectEvent={onSelectEvent}
            eventPropGetter={eventPropGetter}
            components={COMPONENTS}
            formats={formats}
            scrollToTime={SCROLL_TO}
          />
        </MonthEventsContext.Provider>
      </QuickAddContext.Provider>
      {loading && (
        <div className="th-planner-frame__loading">
          <Spin />
        </div>
      )}
    </div>
  );
};
