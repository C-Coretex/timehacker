import { useContext } from 'react';
import type { FC } from 'react';
import type { DateHeaderProps } from 'react-big-calendar';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { MonthEventsContext, monthDayKey } from '../../monthEventsContext';
import './styles.css';

const MAX_DOTS = 3;

/**
 * Month-view day number, drilling into that day. On phones, where the event pills are hidden, it also carries a
 * dot per task and fills the whole cell, so tapping any day opens it. rbc does not wrap this header in a button of
 * its own (unlike the time-grid DayHeader), so the button is ours.
 */
export const MonthDateHeader: FC<DateHeaderProps> = ({ label, date, onDrillDown }) => {
  const { t } = useTranslation();
  const events = useContext(MonthEventsContext).get(monthDayKey(date)) ?? [];

  return (
    <button
      type="button"
      className="rbc-button-link th-month-date"
      aria-label={t('calendar.dayTasks', { date: dayjs(date).format('dddd, D MMMM'), count: events.length })}
      onClick={onDrillDown}
    >
      <span className="th-month-date__label">{label}</span>
      {events.length > 0 && (
        <span className="th-month-date__dots" aria-hidden>
          {events.slice(0, MAX_DOTS).map((event) => (
            <span key={event.id} className={`th-month-date__dot th-month-date__dot--${event.resource?.type ?? 'fixed'}`} />
          ))}
          {events.length > MAX_DOTS && <span className="th-month-date__more">+{events.length - MAX_DOTS}</span>}
        </span>
      )}
    </button>
  );
};
