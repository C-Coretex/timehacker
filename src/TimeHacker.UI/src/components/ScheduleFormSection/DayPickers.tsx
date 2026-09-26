import type { FC } from 'react';
import { DatePicker } from 'antd';
import dayjs from 'dayjs';
import localeData from 'dayjs/plugin/localeData';
import { useTranslation } from 'react-i18next';
import { useSettings } from 'contexts/SettingsContext';

dayjs.extend(localeData);

interface ControlProps<T> {
  value?: T;
  onChange?: (value: T) => void;
  id?: string;
}

const circleClass = (selected: boolean) => `th-day-circle${selected ? ' is-selected' : ''}`;

// dayjs counts Sunday as 0; the API's DayOfWeek is ISO (Monday 1 … Sunday 7).
const toIsoDay = (day: number) => (day === 0 ? 7 : day);

/** Days of the week as toggle circles, in the user's week order. Form value: ISO day numbers. */
export const WeekdayToggles: FC<ControlProps<number[]>> = ({ value = [], onChange, id }) => {
  const { i18n } = useTranslation();
  const { weekStartDay } = useSettings();
  const names = dayjs().locale(i18n.language?.startsWith('ru') ? 'ru' : 'en').localeData().weekdaysMin();
  const days = Array.from({ length: 7 }, (_, offset) => (weekStartDay + offset) % 7);

  const toggle = (isoDay: number) =>
    onChange?.(value.includes(isoDay) ? value.filter((d) => d !== isoDay) : [...value, isoDay].sort((a, b) => a - b));

  return (
    <div className="th-weekdays" id={id}>
      {days.map((day) => {
        const isoDay = toIsoDay(day);
        return (
          <button
            key={isoDay}
            type="button"
            aria-pressed={value.includes(isoDay)}
            className={circleClass(value.includes(isoDay))}
            onClick={() => toggle(isoDay)}
          >
            {names[day]}
          </button>
        );
      })}
    </div>
  );
};

const MONTH_DAYS = Array.from({ length: 31 }, (_, index) => index + 1);

/** The day of the month as a grid of circles, like a calendar page. Form value: 1–31. */
export const MonthDayGrid: FC<ControlProps<number>> = ({ value, onChange, id }) => (
  <div className="th-month-days" role="radiogroup" id={id}>
    {MONTH_DAYS.map((day) => (
      <button
        key={day}
        type="button"
        role="radio"
        aria-checked={day === value}
        className={circleClass(day === value)}
        onClick={() => onChange?.(day)}
      >
        {day}
      </button>
    ))}
  </div>
);

/**
 * A yearly date picked as "15 March". The server stores a day of the year, so the pick is counted in the
 * anchor's year — the one the recurrence starts from.
 */
export const YearDayPicker: FC<ControlProps<number> & { year: number }> = ({ value, onChange, year, id }) => {
  const startOfYear = dayjs().year(year).startOf('year');

  return (
    <DatePicker
      id={id}
      className="th-repeat-year"
      value={value ? startOfYear.add(value - 1, 'day') : null}
      onChange={(date) => date && onChange?.(date.diff(date.startOf('year'), 'day') + 1)}
      format="D MMMM"
      allowClear={false}
    />
  );
};
