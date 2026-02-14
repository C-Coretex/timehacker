import { useState, useEffect, useCallback, useMemo, useRef } from 'react';
import type { FC } from 'react';
import { Calendar, dayjsLocalizer, type View } from 'react-big-calendar';
import dayjs from 'dayjs';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import './calendar-theme.css';
import { Button, Spin, Alert, Modal } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import {
  fetchTasksForDays,
  taskForDayToEvent,
  minutesToDate,
  refreshTasksForDays,
  type CalendarEvent,
} from '../../api/tasks';
import { useTheme } from '../../contexts/ThemeContext';
import { useIsMobile } from '../../hooks/useIsMobile';
import ThreeDayView from './ThreeDayView';

const localizer = dayjsLocalizer(dayjs);

type ExtendedView = View | '3day';

const calendarViews = {
  month: true,
  week: true,
  day: true,
  '3day': ThreeDayView,
};

const CalendarPage: FC = () => {
  const { darkMode } = useTheme();
  const { isMobile } = useIsMobile();
  const initialViewSet = useRef(false);

  const [view, setView] = useState<ExtendedView>(isMobile ? 'day' : 'week');
  const [date, setDate] = useState(new Date());
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);

  // Set default view based on screen size only once on mount
  useEffect(() => {
    if (!initialViewSet.current) {
      initialViewSet.current = true;
      setView(isMobile ? 'day' : 'week');
    }
  }, [isMobile]);

  const handleSelectEvent = useCallback((event: CalendarEvent) => {
    setSelectedEvent(event);
    setIsModalVisible(true);
  }, []);

  const handleViewChange = useCallback((newView: View) => {
    setView(newView as ExtendedView);
  }, []);

  const handleNavigate = useCallback((newDate: Date) => {
    setDate(newDate);
  }, []);

  const handleDrillDown = useCallback((newDate: Date) => {
    setDate(newDate);
    setView('day');
  }, []);

  // Compute date ranges for each view
  const weekStart = useMemo(() => {
    const d = new Date(date);
    d.setDate(d.getDate() - d.getDay());
    d.setHours(0, 0, 0, 0);
    return d;
  }, [date]);

  const weekDays = useMemo(
    () =>
      Array.from({ length: 7 }, (_, i) => {
        const d = new Date(weekStart);
        d.setDate(d.getDate() + i);
        return d;
      }),
    [weekStart]
  );

  const monthDays = useMemo(() => {
    const year = date.getFullYear();
    const month = date.getMonth();
    const lastDay = new Date(year, month + 1, 0).getDate();
    return Array.from({ length: lastDay }, (_, i) => new Date(year, month, i + 1));
  }, [date]);

  const dayDates = useMemo(() => {
    const d = new Date(date);
    d.setHours(0, 0, 0, 0);
    return [d];
  }, [date]);

  const threeDayDates = useMemo(
    () =>
      Array.from({ length: 3 }, (_, i) => {
        const d = new Date(date);
        d.setHours(0, 0, 0, 0);
        d.setDate(d.getDate() + i);
        return d;
      }),
    [date]
  );

  const getDatesForView = useCallback(
    (v: ExtendedView): Date[] => {
      switch (v) {
        case 'month':
          return monthDays;
        case 'week':
          return weekDays;
        case 'day':
          return dayDates;
        case '3day':
          return threeDayDates;
        default:
          return weekDays;
      }
    },
    [monthDays, weekDays, dayDates, threeDayDates]
  );

  const fetchTasks = useCallback(
    async (dates: Date[]) => {
      setLoading(true);
      setError(null);
      try {
        const results = await fetchTasksForDays(dates);
        const allEvents: CalendarEvent[] = [];
        for (const dayResult of results) {
          const dayDate = new Date(dayResult.date);
          for (const item of dayResult.tasksTimeline ?? []) {
            allEvents.push(taskForDayToEvent(item, dayDate, minutesToDate));
          }
        }
        setEvents(allEvents);
      } catch (err: unknown) {
        const message =
          err && typeof err === 'object' && 'response' in err
            ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
            : null;
        setError(message ?? 'Failed to load tasks');
      } finally {
        setLoading(false);
      }
    },
    []
  );

  useEffect(() => {
    fetchTasks(getDatesForView(view));
  }, [view, date, fetchTasks, getDatesForView]);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const dates = getDatesForView(view);
      await refreshTasksForDays(dates);
      await fetchTasks(dates);
    } catch (err: unknown) {
      const message =
        err && typeof err === 'object' && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : null;
      setError(message ?? 'Failed to refresh tasks');
    } finally {
      setLoading(false);
    }
  }, [view, getDatesForView, fetchTasks]);

  const eventStyleGetter = useCallback(
    (event: CalendarEvent) => {
      const colors = darkMode
        ? { default: '#177ddc', fixed: '#49aa19', dynamic: '#d89614' }
        : { default: '#1890ff', fixed: '#52c41a', dynamic: '#faad14' };

      let backgroundColor = colors.default;
      if (event.resource?.type === 'fixed') {
        backgroundColor = colors.fixed;
      } else if (event.resource?.type === 'dynamic') {
        backgroundColor = colors.dynamic;
      }

      return {
        style: {
          backgroundColor,
          borderRadius: '5px',
          opacity: 0.85,
          color: 'white',
          border: '0px',
          display: 'block',
        },
      };
    },
    [darkMode]
  );

  return (
    <div style={{ display: 'flex', flexDirection: 'column', height: '100%', overflow: 'hidden' }}>
      <div style={{ marginBottom: '0.5rem', display: 'flex', gap: '8px' }}>
        <Button icon={<ReloadOutlined />} onClick={refresh} size={isMobile ? 'small' : 'middle'}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert
          type="error"
          message={error}
          showIcon
          style={{ marginBottom: '0.5rem' }}
        />
      )}
      {loading ? (
        <Spin size="large" style={{ display: 'block', margin: '2rem auto' }} />
      ) : (
        <Calendar
          localizer={localizer}
          events={events}
          startAccessor="start"
          endAccessor="end"
          style={{ flex: 1 }}
          view={view as View}
          onView={handleViewChange}
          date={date}
          onNavigate={handleNavigate}
          views={calendarViews}
          onSelectEvent={handleSelectEvent}
          onDrillDown={handleDrillDown}
          eventPropGetter={eventStyleGetter}
          messages={{ '3day': '3 Day' } as Record<string, string>}
        />
      )}

      <Modal
        open={isModalVisible}
        title="Task Details"
        footer={[
          <Button key="close" onClick={() => setIsModalVisible(false)}>
            Close
          </Button>,
        ]}
        onCancel={() => setIsModalVisible(false)}
      >
        {selectedEvent && (
          <div>
            <p>
              <b>Type:</b>{' '}
              {selectedEvent.resource?.type === 'dynamic' ? 'Dynamic' : 'Fixed'}
            </p>
            <p>
              <b>Title:</b> {selectedEvent.title}
            </p>
            <p>
              <b>Description:</b>{' '}
              {selectedEvent.description ?? selectedEvent.resource?.task.description ?? '-'}
            </p>
            <p>
              <b>Priority:</b>{' '}
              {selectedEvent.resource?.task.priority ?? '-'}
            </p>
            <p>
              <b>Start:</b>{' '}
              {dayjs(selectedEvent.start).format('YYYY-MM-DD HH:mm')}
            </p>
            <p>
              <b>End:</b>{' '}
              {dayjs(selectedEvent.end).format('YYYY-MM-DD HH:mm')}
            </p>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default CalendarPage;
