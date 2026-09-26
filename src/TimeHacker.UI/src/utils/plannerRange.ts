import dayjs from 'dayjs';
import type { Dayjs, ManipulateType } from 'dayjs';
import type { CalendarView } from 'contexts/CalendarDateContext';

export interface DateRange {
  start: Dayjs;
  end: Dayjs;
}

const startOfWeek = (day: Dayjs, weekStartDay: number) => day.subtract((day.day() - weekStartDay + 7) % 7, 'day');

/** The days a planner view shows for `date`. Month mirrors rbc's grid: whole weeks around the month. */
export function visibleRange(view: CalendarView, date: Date, weekStartDay: number): DateRange {
  const day = dayjs(date).startOf('day');
  switch (view) {
    case 'day':
      return { start: day, end: day };
    case '3day':
      return { start: day, end: day.add(2, 'day') };
    case 'week': {
      const start = startOfWeek(day, weekStartDay);
      return { start, end: start.add(6, 'day') };
    }
    case 'month':
      return {
        start: startOfWeek(day.startOf('month'), weekStartDay),
        end: startOfWeek(day.endOf('month').startOf('day'), weekStartDay).add(6, 'day'),
      };
  }
}

export const daysIn = ({ start, end }: DateRange): Date[] =>
  Array.from({ length: end.diff(start, 'day') + 1 }, (_, i) => start.add(i, 'day').toDate());

const STEP: Record<CalendarView, [number, ManipulateType]> = {
  day: [1, 'day'],
  '3day': [3, 'day'],
  week: [1, 'week'],
  month: [1, 'month'],
};

/** The date one page back (-1) or forward (+1) in the given view. */
export function shiftDate(view: CalendarView, date: Date, direction: 1 | -1): Date {
  const [amount, unit] = STEP[view];
  return dayjs(date).add(direction * amount, unit).toDate();
}

/** "15–17 July 2024", collapsing whatever the two ends share; month view names just the month. */
export function formatRangeTitle(view: CalendarView, date: Date, { start, end }: DateRange): string {
  if (view === 'month') return dayjs(date).format('MMMM YYYY');
  if (start.isSame(end, 'day')) return start.format('D MMMM YYYY');
  if (start.isSame(end, 'month')) return `${start.format('D')}–${end.format('D MMMM YYYY')}`;
  if (start.isSame(end, 'year')) return `${start.format('D MMM')} – ${end.format('D MMM YYYY')}`;
  return `${start.format('D MMM YYYY')} – ${end.format('D MMM YYYY')}`;
}
