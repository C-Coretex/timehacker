import { useState, useCallback, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { fetchTasksForDays, refreshTasksForDays } from 'api/tasks';
import { taskForDayToEvent, categoriesForDayToEvents } from 'utils/calendarUtils';
import type { CalendarEvent } from 'utils/calendarUtils';
import { getApiErrorMessage } from 'utils/getApiErrorMessage';

/**
 * Owns the calendar's timeline state (events/loading/error) and the fetch/refresh actions. Guards
 * against a stale response from an earlier request overwriting a newer one (e.g. rapid navigation
 * between views/dates).
 *
 * Categories come back in the same response as tasks, so the background layer costs no extra request.
 */
export function useCalendarTasks() {
  const { t } = useTranslation();
  const [events, setEvents] = useState<CalendarEvent[]>([]);
  const [backgroundEvents, setBackgroundEvents] = useState<CalendarEvent[]>([]);
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
        const allBackgroundEvents: CalendarEvent[] = [];
        for (const dayResult of results) {
          const dayDate = new Date(dayResult.date);
          (dayResult.tasksTimeline ?? []).forEach((item, idx) => {
            allEvents.push(taskForDayToEvent(item, dayDate, idx));
          });
          allBackgroundEvents.push(
            ...categoriesForDayToEvents(dayResult.categoriesTimeline ?? [], dayDate)
          );
        }
        setEvents(allEvents);
        setBackgroundEvents(allBackgroundEvents);
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

  return { events, backgroundEvents, loading, error, fetchTasks, refresh };
}
