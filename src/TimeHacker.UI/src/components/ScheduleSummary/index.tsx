import type { FC } from 'react';
import { Spin } from 'antd';
import { SyncOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import type { ScheduleEntityReturnModel } from 'api/types';
import { describeRecurrence } from 'utils/describeRecurrence';
import './styles.css';

interface ScheduleSummaryProps {
  scheduleEntity: ScheduleEntityReturnModel | null;
  loading?: boolean;
}

/** Read-only card for an existing recurrence: what repeats, since when, and until when. */
export const ScheduleSummary: FC<ScheduleSummaryProps> = ({ scheduleEntity, loading = false }) => {
  const { t } = useTranslation();

  if (loading) return <Spin size="small" />;
  if (!scheduleEntity) return null;

  return (
    <div className="th-schedule-summary">
      <div className="th-schedule-summary__rule">
        <SyncOutlined />
        {describeRecurrence(scheduleEntity.repeatingEntity, t)}
      </div>
      <div className="th-schedule-summary__meta">
        <span>
          {t('tasks.scheduleCreated')}: {dayjs(scheduleEntity.scheduleCreated).format('MMM D, YYYY')}
        </span>
        {scheduleEntity.endsOn ? (
          <span className="th-schedule-summary__ends">
            {t('tasks.endsOn')}: {dayjs(scheduleEntity.endsOn).format('MMM D, YYYY')}
          </span>
        ) : (
          <span>{t('tasks.recurringIndefinitely')}</span>
        )}
      </div>
    </div>
  );
};
