import type { FC, SyntheticEvent } from 'react';
import dayjs from 'dayjs';
import { useSettings } from 'contexts/SettingsContext';
import type { CalendarEvent } from 'utils/calendarUtils';

// rbc starts slot selection from a `document` listener that exempts only `.rbc-event`, and a band is a
// `.rbc-background-event`; stopping the press here keeps a click on the name from also starting a quick-add.
const stopSlotSelection = (event: SyntheticEvent) => event.stopPropagation();

/**
 * A category band spans its whole window with tasks drawn over it, so its name is a compact pill pinned to
 * the top-left rail — that is what stays readable on a packed day.
 */
export const CategoryBandLabel: FC<{ event: CalendarEvent }> = ({ event }) => {
  const { timeDisplayFormat } = useSettings();

  return (
    <div className="th-category-label" onMouseDown={stopSlotSelection} onTouchStart={stopSlotSelection}>
      {event.title}
      <span className="th-category-label-time">
        {dayjs(event.start).format(timeDisplayFormat)}&nbsp;&rarr;&nbsp;{dayjs(event.end).format(timeDisplayFormat)}
      </span>
    </div>
  );
};
