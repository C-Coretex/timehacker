import { useState, useEffect, useCallback, useMemo, useRef, memo } from 'react';
import type { FC } from 'react';
import { Calendar, dayjsLocalizer, type View } from 'react-big-calendar';
import dayjs from 'dayjs';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import './calendar-theme.css';
import { Button, Spin, Alert, Modal, notification, Descriptions, Tag, Badge, Space, Divider } from 'antd';
import { PlusOutlined, ReloadOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import {
  fetchTasksForDays,
  taskForDayToEvent,
  minutesToDate,
  refreshTasksForDays,
  type CalendarEvent,
} from '../../api/tasks';
import { createFixedTask, postNewScheduleForTask, fetchFixedTaskById } from '../../api/fixedTasks';
import { createDynamicTask } from '../../api/dynamicTasks';
import { useTheme } from '../../contexts/ThemeContext';
import { useCalendarDate } from '../../contexts/CalendarDateContext';
import { useSettings } from '../../contexts/SettingsContext';
import { useIsMobile } from '../../hooks/useIsMobile';
import ThreeDayView from './ThreeDayView';
import UnifiedTaskFormModal from '../../components/UnifiedTaskFormModal';
import type { ScheduleFormPayload } from '../../components/UnifiedTaskFormModal';
import type { FixedTaskFormData, InputDynamicTask } from '../../api/types';

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
  const { isMobile, screens } = useIsMobile();
  const { t, i18n } = useTranslation();
  const { timeDisplayFormat, weekStart: weekStartSetting } = useSettings();
  const initialViewSet = useRef(false);
  const { selectedDate, setSelectedDate } = useCalendarDate();

  const [view, setView] = useState<ExtendedView>('week');
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [taskModalOpen, setTaskModalOpen] = useState(false);
  const [scheduleData, setScheduleData] = useState<any>(null);
  const [loadingSchedule, setLoadingSchedule] = useState(false);

  // Set default view based on screen size only once after breakpoints resolve
  useEffect(() => {
    if (!initialViewSet.current && screens.md !== undefined) {
      initialViewSet.current = true;
      setView(isMobile ? 'day' : 'week');
    }
  }, [isMobile, screens.md]);

  const handleSelectEvent = useCallback(async (event: CalendarEvent) => {
    setSelectedEvent(event);
    setIsModalVisible(true);
    setScheduleData(null);

    // Fetch schedule data for fixed tasks
    if (event.resource?.isFixed && event.resource.task.id) {
      setLoadingSchedule(true);
      try {
        const taskData = await fetchFixedTaskById(event.resource.task.id);
        setScheduleData(taskData.repeatingEntity);
      } catch (error) {
        console.error('Failed to fetch schedule data:', error);
      } finally {
        setLoadingSchedule(false);
      }
    }
  }, []);

  const handleViewChange = useCallback((newView: View) => {
    setView(newView as ExtendedView);
  }, []);

  const handleNavigate = useCallback((newDate: Date) => {
    setSelectedDate(newDate);
  }, [setSelectedDate]);

  const handleDrillDown = useCallback((newDate: Date) => {
    setSelectedDate(newDate);
    setView('day');
  }, [setSelectedDate]);

  const CustomEvent = useMemo(
    () =>
      memo<{ event: CalendarEvent }>(({ event }) => (
        <div>
          <strong>{event.title}</strong>
          <div style={{ fontSize: '0.75em', opacity: 0.9 }}>
            {dayjs(event.start).format(timeDisplayFormat)} &rarr; {dayjs(event.end).format(timeDisplayFormat)}
          </div>
        </div>
      )),
    [timeDisplayFormat]
  );

  // Compute date ranges for each view
  const weekStartDay = weekStartSetting === 'monday' ? 1 : 0;

  const weekStart = useMemo(() => {
    const d = new Date(selectedDate);
    const day = d.getDay();
    const diff = (day - weekStartDay + 7) % 7;
    d.setDate(d.getDate() - diff);
    d.setHours(0, 0, 0, 0);
    return d;
  }, [selectedDate, weekStartDay]);

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
    () =>
      Array.from({ length: 3 }, (_, i) => {
        const d = new Date(selectedDate);
        d.setHours(0, 0, 0, 0);
        d.setDate(d.getDate() + i);
        return d;
      }),
    [selectedDate]
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
          (dayResult.tasksTimeline ?? []).forEach((item, idx) => {
            allEvents.push(taskForDayToEvent(item, dayDate, minutesToDate, idx));
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
    fetchTasks(getDatesForView(view));
  }, [view, selectedDate, fetchTasks, getDatesForView]);

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
      setError(message ?? t('calendar.refreshFailed'));
    } finally {
      setLoading(false);
    }
  }, [view, getDatesForView, fetchTasks, t]);

  const handleSaveFixed = useCallback(
    async (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => {
      try {
        const payload = {
          name: data.name,
          description: data.description || undefined,
          priority: data.priority,
          startTimestamp: dayjs(data.startTimestamp).format('YYYY-MM-DDTHH:mm:ss'),
          endTimestamp: dayjs(data.endTimestamp).format('YYYY-MM-DDTHH:mm:ss'),
        };

        if (id) {
          // Edit not supported from calendar - only create
        } else {
          const newId = await createFixedTask(payload);
          if (schedule && newId) {
            await postNewScheduleForTask({
              parentEntityId: newId,
              repeatingEntityType: schedule.repeatingEntityType,
              endsOnModel: schedule.endsOnModel ?? undefined,
            });
          }
        }

        setTaskModalOpen(false);
        notification.success({ message: t('tasks.success'), description: t('tasks.fixedTaskAdded') });
        await fetchTasks(getDatesForView(view));
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.fixedTaskSaveFailed') });
      }
    },
    [fetchTasks, getDatesForView, view, t]
  );

  const handleSaveDynamic = useCallback(
    async (data: InputDynamicTask) => {
      try {
        await createDynamicTask(data);
        setTaskModalOpen(false);
        notification.success({ message: t('tasks.success'), description: t('tasks.dynamicTaskAdded') });
        await fetchTasks(getDatesForView(view));
      } catch {
        notification.error({ message: t('tasks.error'), description: t('tasks.dynamicTaskSaveFailed') });
      }
    },
    [fetchTasks, getDatesForView, view, t]
  );

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
    eventTimeRangeFormat: () => '', // We use custom event component
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

      {error && (
        <Alert
          type="error"
          title={error}
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
          date={selectedDate}
          onNavigate={handleNavigate}
          views={calendarViews}
          onSelectEvent={handleSelectEvent}
          onDrillDown={handleDrillDown}
          eventPropGetter={eventStyleGetter}
          culture={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
          messages={calendarMessages as Record<string, string>}
          formats={calendarFormats}
          components={{ event: CustomEvent }}
        />
      )}

      <Modal
        open={isModalVisible}
        title={null}
        footer={null}
        onCancel={() => setIsModalVisible(false)}
        width={600}
      >
        {selectedEvent && (
          <div>
            <Space style={{ marginBottom: 16 }}>
              <Tag
                color={selectedEvent.resource?.type === 'fixed' ? 'green' : 'orange'}
                style={{ fontSize: 14, padding: '4px 12px' }}
              >
                {selectedEvent.resource?.type === 'dynamic' ? t('calendar.dynamic') : t('calendar.fixed')}
              </Tag>
              <Badge
                count={selectedEvent.resource?.task.priority}
                showZero
                color={
                  (selectedEvent.resource?.task.priority ?? 0) >= 8
                    ? '#ff4d4f'
                    : (selectedEvent.resource?.task.priority ?? 0) >= 5
                      ? '#faad14'
                      : '#52c41a'
                }
              />
            </Space>

            <Descriptions
              title={selectedEvent.title}
              column={1}
              bordered
              size="small"
              labelStyle={{ fontWeight: 600, width: '30%' }}
            >
              {selectedEvent.description && (
                <Descriptions.Item label={t('calendar.descriptionLabel')}>
                  {selectedEvent.description}
                </Descriptions.Item>
              )}
              <Descriptions.Item label={t('calendar.priorityLabel')}>
                {selectedEvent.resource?.task.priority ?? '-'}
              </Descriptions.Item>
              <Descriptions.Item label={t('calendar.startLabel')}>
                {dayjs(selectedEvent.start).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
              </Descriptions.Item>
              <Descriptions.Item label={t('calendar.endLabel')}>
                {dayjs(selectedEvent.end).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
              </Descriptions.Item>
            </Descriptions>

            {selectedEvent.resource?.isFixed && (
              <>
                <Divider />
                {loadingSchedule ? (
                  <Spin size="small" />
                ) : scheduleData ? (
                  <Descriptions
                    title={t('taskForm.repeat')}
                    column={1}
                    bordered
                    size="small"
                    labelStyle={{ fontWeight: 600 }}
                  >
                    <Descriptions.Item label={t('taskForm.repeatType')}>
                      {scheduleData.entityType === 1 && (
                        <span>{t('taskForm.repeatsEveryNDays', { count: scheduleData.daysCountToRepeat })}</span>
                      )}
                      {scheduleData.entityType === 2 && (
                        <span>
                          {t('taskForm.repeatsWeeklyOn', {
                            days: scheduleData.repeatsOn
                              .sort()
                              .map((d: number) =>
                                t(`taskForm.${['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'][d - 1]}`)
                              )
                              .join(', '),
                          })}
                        </span>
                      )}
                      {scheduleData.entityType === 3 && (
                        <span>{t('taskForm.repeatsMonthlyOnDay', { day: scheduleData.monthDayToRepeat })}</span>
                      )}
                      {scheduleData.entityType === 4 && (
                        <span>{t('taskForm.repeatsYearlyOnDay', { day: scheduleData.yearDayToRepeat })}</span>
                      )}
                    </Descriptions.Item>
                  </Descriptions>
                ) : null}
              </>
            )}

            <div style={{ marginTop: 16, textAlign: 'right' }}>
              <Button onClick={() => setIsModalVisible(false)}>{t('calendar.close')}</Button>
            </div>
          </div>
        )}
      </Modal>

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
