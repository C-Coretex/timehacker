import { useState, useEffect, useCallback, useMemo } from 'react';
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

const localizer = dayjsLocalizer(dayjs);

const CalendarPage: FC = () => {
  const { darkMode } = useTheme();
  const [view, setView] = useState<View>('week');
  const [date, setDate] = useState(new Date());
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);

  const handleSelectEvent = useCallback((event: CalendarEvent) => {
    setSelectedEvent(event);
    setIsModalVisible(true);
  }, []);

  const handleViewChange = useCallback((newView: View) => {
    setView(newView);
  }, []);

  const handleNavigate = useCallback((newDate: Date) => {
    setDate(newDate);
  }, []);

  const handleDrillDown = useCallback((newDate: Date) => {
    setDate(newDate);
    setView('week');
  }, []);

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

  const fetchWeekTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const results = await fetchTasksForDays(weekDays);
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
      setError(message ?? 'Failed to load tasks for week');
    } finally {
      setLoading(false);
    }
  }, [weekDays]);

  const fetchMonthTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const results = await fetchTasksForDays(monthDays);
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
      setError(message ?? 'Failed to load tasks for month');
    } finally {
      setLoading(false);
    }
  }, [monthDays]);

  useEffect(() => {
    if (view === 'month') {
      fetchMonthTasks();
    } else if (view === 'week') {
      fetchWeekTasks();
    }
  }, [view, date, fetchMonthTasks, fetchWeekTasks]);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const datesToRefresh = view === 'month' ? monthDays : weekDays;
      await refreshTasksForDays(datesToRefresh);
      if (view === 'month') {
        await fetchMonthTasks();
      } else {
        await fetchWeekTasks();
      }
    } catch (err: unknown) {
      const message =
        err && typeof err === 'object' && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : null;
      setError(message ?? 'Failed to refresh tasks');
    } finally {
      setLoading(false);
    }
  }, [view, monthDays, weekDays, fetchMonthTasks, fetchWeekTasks]);

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
        <Button icon={<ReloadOutlined />} onClick={refresh}>
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
          view={view}
          onView={handleViewChange}
          date={date}
          onNavigate={handleNavigate}
          views={['month', 'week']}
          onSelectEvent={handleSelectEvent}
          onDrillDown={handleDrillDown}
          eventPropGetter={eventStyleGetter}
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
