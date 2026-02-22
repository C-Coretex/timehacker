import { useState, useEffect, useCallback, useMemo, useRef } from 'react';
import type { FC } from 'react';
import { Calendar, dayjsLocalizer, type View } from 'react-big-calendar';
import dayjs from 'dayjs';
import updateLocale from 'dayjs/plugin/updateLocale';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import './calendar-theme.css';
import { Alert, Button, notification, Spin } from 'antd';
import { PlusOutlined, ReloadOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { fetchTasksForDays, refreshTasksForDays } from '../../api/tasks';
import { createFixedTask, postNewScheduleForTask, fetchFixedTaskById } from '../../api/fixedTasks';
import { createDynamicTask } from '../../api/dynamicTasks';
import { taskForDayToEvent } from '../../utils/calendarUtils';
import type { CalendarEvent } from '../../utils/calendarUtils';
import type { ScheduleEntityReturnModel } from '../../api/types';
import { useTheme } from '../../contexts/ThemeContext';
import { useCalendarDate } from '../../contexts/CalendarDateContext';
import type { CalendarView } from '../../contexts/CalendarDateContext';
import { useSettings } from '../../contexts/SettingsContext';
import { useIsMobile } from '../../hooks/useIsMobile';
import ThreeDayView from './ThreeDayView';
import UnifiedTaskFormModal from '../../components/UnifiedTaskFormModal';
import type { ScheduleFormPayload } from '../../components/UnifiedTaskFormModal';
import type { FixedTaskFormData, InputDynamicTask } from '../../api/types';
import CustomCalendarEvent from './components/CustomCalendarEvent';
import EventDetailModal from './components/EventDetailModal';

dayjs.extend(updateLocale);

const calendarViews = {
  month: true,
  week: true,
  day: true,
  '3day': ThreeDayView,
};

const CalendarPage: FC = () => {
  const { darkMode } = useTheme();
  const { isMobile, screens } = useIsMobile();
  const { t, i18n } = useTranslation();
  const { timeDisplayFormat, weekStart: weekStartSetting } = useSettings();
  const initialViewSet = useRef(false);
  const { selectedDate, setSelectedDate, calendarView, setCalendarView } = useCalendarDate();
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [taskModalOpen, setTaskModalOpen] = useState(false);
  const [scheduleData, setScheduleData] = useState<ScheduleEntityReturnModel | null>(null);
  const [loadingSchedule, setLoadingSchedule] = useState(false);

  const weekStartDay = weekStartSetting === 'monday' ? 1 : 0;

  const localizer = useMemo(() => {
    dayjs.updateLocale('en', { weekStart: weekStartDay });
    return dayjsLocalizer(dayjs);
  }, [weekStartDay]);

  useEffect(() => {
    if (!initialViewSet.current && screens.md !== undefined) {
      initialViewSet.current = true;
      setCalendarView(isMobile ? 'day' : 'week');
    }
  }, [isMobile, screens.md]);

  // --- Date ranges per view ---

  const weekStart = useMemo(() => {
    const d = new Date(selectedDate);
    const diff = (d.getDay() - weekStartDay + 7) % 7;
    d.setDate(d.getDate() - diff);
    d.setHours(0, 0, 0, 0);
    return d;
  }, [selectedDate, weekStartDay]);

  const weekDays = useMemo(
    () => Array.from({ length: 7 }, (_, i) => {
      const d = new Date(weekStart);
      d.setDate(d.getDate() + i);
      return d;
    }),
    [weekStart]
  );

  const monthDays = useMemo(() => {
    const year = selectedDate.getFullYear();
    const month = selectedDate.getMonth();
    const lastDay = new Date(year, month + 1, 0).getDate();
    return Array.from({ length: lastDay }, (_, i) => new Date(year, month, i + 1));
  }, [selectedDate]);

  const dayDates = useMemo(() => {
    const d = new Date(selectedDate);
    d.setHours(0, 0, 0, 0);
    return [d];
  }, [selectedDate]);

  const threeDayDates = useMemo(
    () => Array.from({ length: 3 }, (_, i) => {
      const d = new Date(selectedDate);
      d.setHours(0, 0, 0, 0);
      d.setDate(d.getDate() + i);
      return d;
    }),
    [selectedDate]
  );

  const getDatesForView = useCallback(
    (v: CalendarView): Date[] => {
      switch (v) {
        case 'month': return monthDays;
        case 'week': return weekDays;
        case 'day': return dayDates;
        case '3day': return threeDayDates;
        default: return weekDays;
      }
    },
    [monthDays, weekDays, dayDates, threeDayDates]
  );

  // --- Data fetching ---

  const fetchTasks = useCallback(
    async (dates: Date[]) => {
      setLoading(true);
      setError(null);
      try {
        const results = await fetchTasksForDays(dates);
        const allEvents: CalendarEvent[] = [];
        for (const dayResult of results) {
          const dayDate = new Date(dayResult.date);
          (dayResult.tasksTimeline ?? []).forEach((item, idx) => {
            allEvents.push(taskForDayToEvent(item, dayDate, idx));
          });
        }
        setEvents(allEvents);
      } catch (err: unknown) {
        const message =
          err && typeof err === 'object' && 'response' in err
            ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
            : null;
        setError(message ?? t('calendar.loadFailed'));
      } finally {
        setLoading(false);
      }
    },
    [t]
  );

  useEffect(() => {
    fetchTasks(getDatesForView(calendarView));
  }, [calendarView, selectedDate, fetchTasks, getDatesForView]);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const dates = getDatesForView(calendarView);
      await refreshTasksForDays(dates);
      await fetchTasks(dates);
    } catch (err: unknown) {
      const message =
        err && typeof err === 'object' && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : null;
      setError(message ?? t('calendar.refreshFailed'));
    } finally {
      setLoading(false);
    }
  }, [calendarView, getDatesForView, fetchTasks, t]);

  // --- Event handlers ---

  const handleSelectEvent = useCallback(async (event: CalendarEvent) => {
    setSelectedEvent(event);
    setIsModalVisible(true);
    setScheduleData(null);

    if (event.resource?.isFixed && event.resource.task.id) {
      setLoadingSchedule(true);
      try {
        const taskData = await fetchFixedTaskById(event.resource.task.id);
        setScheduleData(taskData.scheduleEntity);
      } catch {
        // schedule data is optional; failure is non-blocking
      } finally {
        setLoadingSchedule(false);
      }
    }
  }, []);

  const handleSaveFixed = useCallback(
    async (data: FixedTaskFormData, _id?: string, schedule?: ScheduleFormPayload) => {
      try {
        const payload = {
          name: data.name,
          description: data.description || undefined,
          priority: data.priority,
          startTimestamp: dayjs(data.startTimestamp).format('YYYY-MM-DDTHH:mm:ss'),
          endTimestamp: dayjs(data.endTimestamp).format('YYYY-MM-DDTHH:mm:ss'),
        };
        const newId = await createFixedTask(payload);
        if (schedule && newId) {
          await postNewScheduleForTask({
            parentEntityId: newId,
            repeatingEntityType: schedule.repeatingEntityType,
            endsOnModel: schedule.endsOnModel ?? undefined,
          });
        }
        setTaskModalOpen(false);
        notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskAdded') });
        await fetchTasks(getDatesForView(calendarView));
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.fixedTaskSaveFailed') });
      }
    },
    [fetchTasks, getDatesForView, calendarView, t]
  );

  const handleSaveDynamic = useCallback(
    async (data: InputDynamicTask) => {
      try {
        await createDynamicTask(data);
        setTaskModalOpen(false);
        notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskAdded') });
        await fetchTasks(getDatesForView(calendarView));
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.dynamicTaskSaveFailed') });
      }
    },
    [fetchTasks, getDatesForView, calendarView, t]
  );

  const eventStyleGetter = useCallback(
    (event: CalendarEvent) => {
      const colors = darkMode
        ? { default: '#177ddc', fixed: '#49aa19', dynamic: '#d89614' }
        : { default: '#1890ff', fixed: '#52c41a', dynamic: '#faad14' };
      const backgroundColor =
        event.resource?.type === 'fixed' ? colors.fixed
        : event.resource?.type === 'dynamic' ? colors.dynamic
        : colors.default;
      return {
        style: {
          backgroundColor,
          borderRadius: '6px',
          opacity: 0.85,
          color: 'white',
          border: '0px',
          display: 'block',
          padding: '2px 6px',
        },
      };
    },
    [darkMode]
  );

  const calendarMessages = useMemo(() => ({
    today: t('calendar.today'),
    previous: t('calendar.previous'),
    next: t('calendar.next'),
    month: t('calendar.month'),
    week: t('calendar.week'),
    day: t('calendar.day'),
    '3day': t('calendar.threeDayView'),
  }), [t]);

  const calendarFormats = useMemo(() => ({
    timeGutterFormat: timeDisplayFormat,
    eventTimeRangeFormat: () => '',
    agendaTimeRangeFormat: () => '',
  }), [timeDisplayFormat]);

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      <div style={{ marginBottom: '0.5rem', display: 'flex', justifyContent: 'space-between', gap: '8px' }}>
        <Button icon={<ReloadOutlined />} onClick={refresh} size={isMobile ? 'small' : 'middle'}>
          {t('calendar.refresh')}
        </Button>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setTaskModalOpen(true)}
          size={isMobile ? 'small' : 'middle'}
        >
          {t('calendar.addTask')}
        </Button>
      </div>

      {error && <Alert type="error" title={error} showIcon style={{ marginBottom: '0.5rem' }} />}

      {loading ? (
        <Spin size="large" style={{ display: 'block', margin: '2rem auto' }} />
      ) : (
        <Calendar
          localizer={localizer}
          events={events}
          startAccessor="start"
          endAccessor="end"
          style={{ flex: 1 }}
          view={calendarView as View}
          onView={(v) => setCalendarView(v as CalendarView)}
          date={selectedDate}
          onNavigate={setSelectedDate}
          views={calendarViews}
          onSelectEvent={handleSelectEvent}
          onDrillDown={(date) => { setSelectedDate(date); setCalendarView('day'); }}
          eventPropGetter={eventStyleGetter}
          culture={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
          messages={calendarMessages as Record<string, string>}
          formats={calendarFormats}
          components={{ event: CustomCalendarEvent }}
        />
      )}

      <EventDetailModal
        open={isModalVisible}
        onClose={() => setIsModalVisible(false)}
        event={selectedEvent}
        scheduleData={scheduleData}
        loadingSchedule={loadingSchedule}
        timeDisplayFormat={timeDisplayFormat}
      />

      <UnifiedTaskFormModal
        open={taskModalOpen}
        onCancel={() => setTaskModalOpen(false)}
        onSaveFixed={handleSaveFixed}
        onSaveDynamic={handleSaveDynamic}
        defaultDate={selectedDate}
      />
    </div>
  );
};

export default CalendarPage;
