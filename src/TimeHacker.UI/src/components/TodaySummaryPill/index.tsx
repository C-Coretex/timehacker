import type { FC } from 'react';
import { useTranslation } from 'react-i18next';
import type { DaySummary } from 'utils/daySummary';
import './styles.css';

/** "You have N more tasks in list for today", with a nudge when some of them are high priority. */
export const TodaySummaryPill: FC<{ summary: DaySummary }> = ({ summary }) => {
  const { t } = useTranslation();

  return (
    <span className="th-summary-pill">
      {summary.remaining > 0
        ? t('calendar.remainingToday', { count: summary.remaining })
        : t('calendar.nothingLeftToday')}
      {summary.highPriorityRemaining > 0 && (
        <span className="th-summary-pill__urgent">
          {t('calendar.highPriorityLeft', { count: summary.highPriorityRemaining })}
        </span>
      )}
    </span>
  );
};
