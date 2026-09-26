import { useCallback, useRef, useState } from 'react';
import { fetchFixedTaskById } from 'api/fixedTasks';
import type { ScheduleEntityReturnModel } from 'api/types';
import type { CalendarEvent } from 'utils/calendarUtils';

/**
 * The planner item whose details are open. A fixed task's recurrence is not on the timeline, so it is
 * fetched when the item opens; a response for an item that has since been replaced is dropped.
 */
export function useEventDetails() {
  const [isOpen, setIsOpen] = useState(false);
  const [event, setEvent] = useState<CalendarEvent | null>(null);
  const [scheduleEntity, setScheduleEntity] = useState<ScheduleEntityReturnModel | null>(null);
  const [loadingSchedule, setLoadingSchedule] = useState(false);
  const latestRequest = useRef(0);

  const open = useCallback(async (next: CalendarEvent) => {
    const request = ++latestRequest.current;
    setEvent(next);
    setIsOpen(true);
    setScheduleEntity(null);
    if (next.resource?.type !== 'fixed') {
      setLoadingSchedule(false);
      return;
    }

    setLoadingSchedule(true);
    try {
      const task = await fetchFixedTaskById(next.resource.task.id);
      if (request === latestRequest.current) setScheduleEntity(task.scheduleEntity);
    } catch {
      // The recurrence is extra detail; the rest of the dialog is already correct without it.
    } finally {
      if (request === latestRequest.current) setLoadingSchedule(false);
    }
  }, []);

  // `event` is kept so the dialog does not empty itself while its close animation plays.
  const close = useCallback(() => setIsOpen(false), []);

  return { isOpen, event, scheduleEntity, loadingSchedule, open, close };
}
