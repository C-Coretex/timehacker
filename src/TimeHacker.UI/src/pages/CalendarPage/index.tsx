import { useCallback, useEffect, useMemo, useRef } from 'react';
import type { FC } from 'react';
import type { SlotInfo } from 'react-big-calendar';
import { Alert } from 'antd';
import dayjs from 'dayjs';
import { EventDetailModal } from 'components/EventDetailModal';
import { TodaySummaryPill } from 'components/TodaySummaryPill';
import { UnifiedTaskFormModal } from 'components/UnifiedTaskFormModal';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import { useCalendarTasks } from 'hooks/useCalendarTasks';
import { useEventDetails } from 'hooks/useEventDetails';
import { useIsMobile } from 'hooks/useIsMobile';
import { useTaskCategories } from 'hooks/useTaskCategories';
import { useTaskComposer } from 'hooks/useTaskComposer';
import { attachCategoriesToEvents } from 'utils/calendarUtils';
import { summarizeDay } from 'utils/daySummary';
import { daysIn, formatRangeTitle, shiftDate, visibleRange } from 'utils/plannerRange';
import { PlannerToolbar } from './components/PlannerToolbar';
import { WeekStrip } from './components/WeekStrip';
import { PlannerCalendar } from './PlannerCalendar';
import './styles.css';

/** The planner: tasks and category windows over a D/3D/W/M time grid, with quick-add on empty slots. */
export const CalendarPage: FC = () => {
  const { weekStartDay } = useSettings();
  const { isMobile, screens } = useIsMobile();
  const { selectedDate, setSelectedDate, calendarView, setCalendarView } = useCalendarDate();
  const { events, backgroundEvents, loading, error, fetchTasks, refresh } = useCalendarTasks();
  const { categoriesByTaskId, fetchTaskCategories } = useTaskCategories();
  const details = useEventDetails();

  const range = useMemo(
    () => visibleRange(calendarView, selectedDate, weekStartDay),
    [calendarView, selectedDate, weekStartDay]
  );
  const days = useMemo(() => daysIn(range), [range]);
  const upcomingDays = days.filter((day) => !dayjs(day).isBefore(dayjs(), 'day'));
  const planned = useMemo(() => attachCategoriesToEvents(events, categoriesByTaskId), [events, categoriesByTaskId]);

  const reload = useCallback(() => Promise.all([fetchTasks(days), fetchTaskCategories()]), [days, fetchTasks, fetchTaskCategories]);
  const composer = useTaskComposer(reload);

  // Phones open on a single day, desktops on the week — decided once, when the breakpoint is first known.
  const initialViewSet = useRef(false);
  useEffect(() => {
    if (initialViewSet.current || screens.md === undefined) return;
    initialViewSet.current = true;
    setCalendarView(isMobile ? 'day' : 'week');
  }, [isMobile, screens.md, setCalendarView]);

  useEffect(() => {
    void fetchTasks(days);
  }, [days, fetchTasks]);

  const openHour = (hourStart: Date) => {
    const from = dayjs(hourStart).startOf('hour');
    composer.open({ date: from, start: from, end: from.add(1, 'hour') });
  };

  const handleSelectSlot = ({ start, end, action }: SlotInfo) => {
    if (action === 'doubleClick') return;
    if (calendarView === 'month') {
      composer.open({ date: dayjs(start) });
      return;
    }
    // The hour's "+" is the only click target. A drag (or a long press on touch) still selects a range.
    if (action === 'click') return;
    composer.open({ date: dayjs(start), start: dayjs(start), end: dayjs(end) });
  };

  const showsToday = !dayjs().isBefore(range.start, 'day') && !dayjs().isAfter(range.end, 'day');

  return (
    <div className="th-planner-page">
      <PlannerToolbar
        title={formatRangeTitle(calendarView, selectedDate, range)}
        view={calendarView}
        onViewChange={setCalendarView}
        onPrevious={() => setSelectedDate(shiftDate(calendarView, selectedDate, -1))}
        onNext={() => setSelectedDate(shiftDate(calendarView, selectedDate, 1))}
        onToday={() => setSelectedDate(new Date())}
        isToday={dayjs(selectedDate).isSame(dayjs(), 'day')}
        onReplan={() => void refresh(upcomingDays, days)}
        canReplan={upcomingDays.length > 0}
        busy={loading}
        onAddTask={() => composer.open({ date: dayjs(selectedDate) })}
      />

      {calendarView === 'day' && (
        <div className="th-planner-page__phone-strip">
          <WeekStrip />
          {showsToday && <TodaySummaryPill summary={summarizeDay(planned, new Date())} />}
        </div>
      )}

      {error && <Alert type="error" title={error} showIcon className="th-planner-page__error" />}

      <PlannerCalendar
        events={planned}
        backgroundEvents={backgroundEvents}
        loading={loading}
        onSelectEvent={details.open}
        onSelectSlot={handleSelectSlot}
        onQuickAdd={openHour}
      />

      <EventDetailModal
        open={details.isOpen}
        onClose={details.close}
        event={details.event}
        scheduleEntity={details.scheduleEntity}
        loadingSchedule={details.loadingSchedule}
      />

      <UnifiedTaskFormModal
        open={composer.isOpen}
        prefill={composer.prefill}
        onCancel={composer.close}
        onSaveFixed={composer.saveFixed}
        onSaveDynamic={composer.saveDynamic}
      />
    </div>
  );
};
