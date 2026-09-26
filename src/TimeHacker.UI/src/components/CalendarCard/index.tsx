import { useState } from 'react';
import type { FC } from 'react';
import { Calendar } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useCalendarLocale } from 'hooks/useCalendarLocale';
import { CalendarCardDateRow } from './DateRow';
import { CalendarCardHeader } from './Header';
import type { CalendarCardProps } from './types';
import './styles.css';

const classNames = (...names: (string | false | undefined)[]) => names.filter(Boolean).join(' ');

/**
 * The design's month card (« ‹ month › », bold weekdays, navy selection). It owns the month being browsed,
 * so paging through months never changes the selected day — antd's own header would select as it pages.
 */
export const CalendarCard: FC<CalendarCardProps> = ({ value, onChange, highlight, footer, collapsible = false, className }) => {
  const locale = useCalendarLocale();
  const [panel, setPanel] = useState<Dayjs>(() => value ?? dayjs());
  const [followedValue, setFollowedValue] = useState(value);
  // Only phones read this (CSS hides the grid while folded); desktop always shows the grid.
  const [expanded, setExpanded] = useState(false);

  // Follow a selection made elsewhere (the planner, a form reset) by jumping to its month.
  if (value && (!followedValue || !value.isSame(followedValue, 'day'))) {
    setFollowedValue(value);
    setPanel(value);
  }

  const inRange = (day: Dayjs) =>
    !!highlight && !day.isBefore(highlight.start, 'day') && !day.isAfter(highlight.end, 'day');

  // The range is one band per grid row, so it rounds off where the range or the week ends.
  const cellClassName = (day: Dayjs) =>
    classNames(
      'th-calendar-card__cell',
      inRange(day) && 'is-in-range',
      inRange(day) && (day.isSame(highlight?.start, 'day') || day.isSame(day.startOf('week'), 'day')) && 'is-range-start',
      inRange(day) && (day.isSame(highlight?.end, 'day') || day.isSame(day.endOf('week'), 'day')) && 'is-range-end'
    );

  const dayClassName = (day: Dayjs) =>
    classNames(
      'th-calendar-card__day',
      !day.isSame(panel, 'month') && 'is-outside',
      day.isSame(dayjs(), 'day') && 'is-today',
      value?.isSame(day, 'day') && 'is-selected'
    );

  return (
    <div
      className={classNames('th-calendar-card', collapsible && 'th-calendar-card--collapsible', expanded && 'is-expanded', className)}
    >
      {collapsible && <CalendarCardDateRow value={value} expanded={expanded} onToggle={() => setExpanded((open) => !open)} />}
      <Calendar
        fullscreen={false}
        value={panel}
        locale={locale}
        onSelect={(date, { source }) => {
          setPanel(date);
          if (source !== 'date') return;
          onChange?.(date);
          setExpanded(false);
        }}
        headerRender={() => <CalendarCardHeader panel={panel} onNavigate={setPanel} />}
        fullCellRender={(day, info) =>
          info.type === 'date' ? (
            <div className={cellClassName(day)}>
              <div className={dayClassName(day)}>{day.date()}</div>
            </div>
          ) : (
            info.originNode
          )
        }
      />
      {footer && <div className="th-calendar-card__footer">{footer}</div>}
    </div>
  );
};
