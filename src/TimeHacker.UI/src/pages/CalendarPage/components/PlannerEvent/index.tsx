import { memo } from 'react';
import type { CalendarEvent } from 'utils/calendarUtils';
import { CategoryBandLabel } from './CategoryBandLabel';
import { TaskEventCard } from './TaskEventCard';
import './styles.css';

/** Body of every planner item: a category band's name pill, or a task card that adapts to its size. */
export const PlannerEvent = memo<{ event: CalendarEvent }>(({ event }) =>
  event.resource?.type === 'category' ? <CategoryBandLabel event={event} /> : <TaskEventCard event={event} />
);

PlannerEvent.displayName = 'PlannerEvent';
