import type { CSSProperties } from 'react';
import type { CalendarEvent } from 'utils/calendarUtils';
import { argbToHex } from 'utils/colorArgb';
import { priorityEmphasis } from 'utils/priority';

/**
 * Classes and colour variables per planner item. rbc overwrites top/height/width/left inline from its own
 * layout, so only colour and a class travel through here; `calendar-theme.css` and `PlannerEvent/styles.css`
 * do the rest, including every band's geometry.
 */
export function eventPropGetter(event: CalendarEvent): { className?: string; style?: CSSProperties } {
  const resource = event.resource;
  if (!resource) return {};

  if (resource.type === 'category') {
    return {
      className: `th-category-band th-category-depth-${Math.min(resource.depth, 3)}`,
      style: { '--th-category-color': argbToHex(resource.category.color) } as CSSProperties,
    };
  }

  const [firstCategory] = resource.categories;
  const emphasis = priorityEmphasis(resource.task.priority);
  return {
    className: `th-event-host th-event-host--${resource.type}${emphasis ? ` th-event-host--${emphasis}` : ''}`,
    style: firstCategory ? ({ '--th-event-accent': argbToHex(firstCategory.color) } as CSSProperties) : undefined,
  };
}
