import { useMemo } from 'react';
import dayjs from 'dayjs';
import localeData from 'dayjs/plugin/localeData';
import { useTranslation } from 'react-i18next';
import enUS from 'antd/es/calendar/locale/en_US';
import ruRU from 'antd/es/calendar/locale/ru_RU';

dayjs.extend(localeData);

/** antd Calendar locale for the active language, with the design's three-letter weekday headers ("Mon"). */
export function useCalendarLocale() {
  const { i18n } = useTranslation();
  const isRussian = i18n.language?.startsWith('ru') ?? false;

  return useMemo(() => {
    const base = isRussian ? ruRU : enUS;
    const shortWeekDays = dayjs().locale(isRussian ? 'ru' : 'en').localeData().weekdaysShort();
    return { ...base, lang: { ...base.lang, shortWeekDays } };
  }, [isRussian]);
}
