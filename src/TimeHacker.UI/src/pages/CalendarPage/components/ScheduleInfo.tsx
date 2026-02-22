import type { FC } from 'react';
import { Divider, Space, Spin, Typography } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import type { ScheduleEntityReturnModel } from '../../../api/types';
import { RepeatingEntityTypeEnum } from '../../../api/types';

interface Props {
  scheduleData: ScheduleEntityReturnModel | null;
  loading: boolean;
}

const DAY_KEYS = ['mon', 'tue', 'wed', 'thu', 'fri', 'sat', 'sun'] as const;

const ScheduleInfo: FC<Props> = ({ scheduleData, loading }) => {
  const { t } = useTranslation();

  if (loading) return <Spin size="small" />;
  if (!scheduleData) return null;

  const { repeatingEntity } = scheduleData;

  const repeatDescription = (() => {
    switch (repeatingEntity.entityType) {
      case RepeatingEntityTypeEnum.DayRepeatingEntity:
        return t('taskForm.repeatsEveryNDays', { count: repeatingEntity.daysCountToRepeat });
      case RepeatingEntityTypeEnum.WeekRepeatingEntity:
        return t('taskForm.repeatsWeeklyOn', {
          days: repeatingEntity.repeatsOn
            .sort()
            .map((d) => t(`taskForm.${DAY_KEYS[d - 1]}`))
            .join(', '),
        });
      case RepeatingEntityTypeEnum.MonthRepeatingEntity:
        return t('taskForm.repeatsMonthlyOnDay', { day: repeatingEntity.monthDayToRepeat });
      case RepeatingEntityTypeEnum.YearRepeatingEntity:
        return t('taskForm.repeatsYearlyOnDay', { day: repeatingEntity.yearDayToRepeat });
    }
  })();

  return (
    <div className="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
      <Space direction="vertical" size="middle" style={{ width: '100%' }}>
        <div>
          <span style={{ fontWeight: 600 }}>{t('tasks.recurringSchedule')}</span>
          <div style={{ marginTop: 8 }}>{repeatDescription}</div>
        </div>

        <Divider style={{ margin: '8px 0' }} />

        <Space direction="vertical" size="small" style={{ width: '100%' }}>
          <span style={{ fontSize: 13, opacity: 0.7 }}>
            {t('tasks.scheduleCreated')}: {dayjs(scheduleData.scheduleCreated).format('MMM D, YYYY HH:mm')}
          </span>

          {scheduleData.endsOn ? (
            <Typography.Text type="warning" style={{ fontSize: 13, fontWeight: 600 }}>
              {t('tasks.endsOn')}: {dayjs(scheduleData.endsOn).format('MMM D, YYYY')}
            </Typography.Text>
          ) : (
            <span style={{ fontSize: 13, opacity: 0.7 }}>{t('tasks.recurringIndefinitely')}</span>
          )}
        </Space>
      </Space>
    </div>
  );
};

export default ScheduleInfo;
