import React, { useState, useEffect, useCallback, useMemo } from 'react';
import type { FC } from 'react';
import { Calendar, momentLocalizer, type View } from 'react-big-calendar';
import moment from 'moment';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { Typography, Button, Spin, Alert, Modal } from 'antd';
import { ReloadOutlined } from '@ant-design/icons';
import api from '../../api/api';
import {
  fetchTasksForDay,
  taskForDayToEvent,
  minutesToDate,
  refreshTasksForDays,
  type CalendarEvent,
} from '../../api/tasks';

const { Title } = Typography;

const localizer = momentLocalizer(moment);

const CalendarPage: FC = () => {
  const [view, setView] = useState<View>('month');
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

  const fetchFixedTasksForMonth = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.get<Array<{
        id: string;
        name: string;
        description: string;
        startTimestamp: string;
        endTimestamp: string;
      }>>('/api/FixedTasks/GetAll');
      const tasks = response.data.map((task) => ({
        id: task.id,
        title: task.name,
        start: new Date(task.startTimestamp),
        end: new Date(task.endTimestamp),
        allDay: false,
        description: task.description,
        resource: {
          type: 'fixed' as const,
          isFixed: true,
          task: {
            id: task.id,
            name: task.name,
            description: task.description,
            priority: 0,
          },
          start: new Date(task.startTimestamp),
          end: new Date(task.endTimestamp),
        },
      })) as CalendarEvent[];
      setEvents(tasks);
    } catch (err: unknown) {
      const message =
        err && typeof err === 'object' && 'response' in err
          ? (err as { response?: { data?: { message?: string } } }).response?.data?.message
          : null;
      setError(message ?? 'Failed to load tasks');
    } finally {
      setLoading(false);
    }
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
      const days = Array.from({ length: 7 }, (_, i) => {
        const d = new Date(weekStart);
        d.setDate(d.getDate() + i);
        return d;
      });
      const results = await Promise.all(days.map((d) => fetchTasksForDay(d)));
      const allEvents: CalendarEvent[] = [];
      for (let i = 0; i < results.length; i++) {
        const dayDate = days[i];
        const timeline = results[i].tasksTimeline ?? [];
        for (const item of timeline) {
          const ev = taskForDayToEvent(item, dayDate, (d, mins) =>
            minutesToDate(d, mins)
          );
          allEvents.push(ev);
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
  }, [weekStart]);

  useEffect(() => {
    if (view === 'month') {
      fetchFixedTasksForMonth();
    } else if (view === 'week') {
      fetchWeekTasks();
    }
  }, [view, date, fetchFixedTasksForMonth, fetchWeekTasks]);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const datesToRefresh = view === 'month' ? monthDays : weekDays;
      await refreshTasksForDays(datesToRefresh);
      if (view === 'month') {
        await fetchFixedTasksForMonth();
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
  }, [view, monthDays, weekDays, fetchFixedTasksForMonth, fetchWeekTasks]);

  return (
    <div style={{ padding: '1rem' }}>
      <Title level={2}>Tasks Calendar</Title>

      <div style={{ marginBottom: '1rem', display: 'flex', gap: '8px' }}>
        <Button icon={<ReloadOutlined />} onClick={refresh}>
          Refresh
        </Button>
      </div>

      {error && (
        <Alert
          type="error"
          message={error}
          showIcon
          style={{ marginBottom: '1rem' }}
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
          style={{ height: 500 }}
          view={view}
          onView={handleViewChange}
          date={date}
          onNavigate={handleNavigate}
          views={['month', 'week']}
          onSelectEvent={handleSelectEvent}
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
              {moment(selectedEvent.start).format('YYYY-MM-DD HH:mm')}
            </p>
            <p>
              <b>End:</b>{' '}
              {moment(selectedEvent.end).format('YYYY-MM-DD HH:mm')}
            </p>
          </div>
        )}
      </Modal>
    </div>
  );
};

export default CalendarPage;
