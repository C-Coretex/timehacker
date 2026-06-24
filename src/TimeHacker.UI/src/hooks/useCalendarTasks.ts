import { useState, useCallback, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { fetchTasksForDays, refreshTasksForDays } from 'api/tasks';
import { taskForDayToEvent } from 'utils/calendarUtils';
import type { CalendarEvent } from 'utils/calendarUtils';
import { getApiErrorMessage } from 'utils/getApiErrorMessage';

/**
 * Owns the calendar's task-timeline state (events/loading/error) and the
 * fetch/refresh actions. Guards against a stale response from an earlier request
 * overwriting a newer one (e.g. rapid navigation between views/dates).
 */
export function useCalendarTasks() {
  const { t } = useTranslation();
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const latestRequestId = useRef(0);

  const fetchTasks = useCallback(
    async (dates: Date[]) => {
      const requestId = ++latestRequestId.current;
      setLoading(true);
      setError(null);
      try {
        const results = await fetchTasksForDays(dates);
        if (requestId !== latestRequestId.current) return; // a newer request superseded this one
        const allEvents: CalendarEvent[] = [];
        for (const dayResult of results) {
          const dayDate = new Date(dayResult.date);
          (dayResult.tasksTimeline ?? []).forEach((item, idx) => {
            allEvents.push(taskForDayToEvent(item, dayDate, idx));
          });
        }
        setEvents(allEvents);
      } catch (err: unknown) {
        if (requestId !== latestRequestId.current) return;
        setError(getApiErrorMessage(err) ?? t('calendar.loadFailed'));
      } finally {
        if (requestId === latestRequestId.current) setLoading(false);
      }
    },
    [t]
  );

  const refresh = useCallback(
    async (dates: Date[]) => {
      setLoading(true);
      setError(null);
      try {
        await refreshTasksForDays(dates);
        await fetchTasks(dates);
      } catch (err: unknown) {
        setError(getApiErrorMessage(err) ?? t('calendar.refreshFailed'));
        setLoading(false);
      }
    },
    [fetchTasks, t]
  );

  return { events, loading, error, fetchTasks, refresh };
}
