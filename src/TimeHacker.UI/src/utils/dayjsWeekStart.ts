import dayjs from 'dayjs';
import updateLocale from 'dayjs/plugin/updateLocale';
import 'dayjs/locale/ru';

dayjs.extend(updateLocale);

// Every locale the UI ships; antd pickers and react-big-calendar each read the week start of the active one.
const SHIPPED_LOCALES = ['en', 'ru'] as const;

/** dayjs keeps the first day of the week per locale, so it is set on all of them to keep every calendar in step. */
export function applyWeekStart(weekStartDay: number): void {
  SHIPPED_LOCALES.forEach((locale) => dayjs.updateLocale(locale, { weekStart: weekStartDay }));
}
