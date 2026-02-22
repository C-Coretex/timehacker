import type { FC } from 'react';
import { Divider, Space, Typography } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import type { ScheduleEntityReturnModel } from '../../api/types';
import { RepeatingEntityTypeEnum } from '../../api/types';
import { getDaysOfWeek } from './constants';

interface Props {
  scheduleEntity: ScheduleEntityReturnModel;
}

const ScheduleReadOnlyInfo: FC<Props> = ({ scheduleEntity }) => {
  const { t } = useTranslation();
  const daysOfWeek = getDaysOfWeek(t);
  const { repeatingEntity } = scheduleEntity;

  const repeatDescription = (() => {
    switch (repeatingEntity.entityType) {
      case RepeatingEntityTypeEnum.DayRepeatingEntity:
        return t('taskForm.repeatsEveryNDays', { count: repeatingEntity.daysCountToRepeat });
      case RepeatingEntityTypeEnum.WeekRepeatingEntity:
        return t('taskForm.repeatsWeeklyOn', {
          days: repeatingEntity.repeatsOn
            .map((d) => daysOfWeek.find((dw) => dw.value === d)?.label ?? String(d))
            .join(', '),
        });
      case RepeatingEntityTypeEnum.MonthRepeatingEntity:
        return t('taskForm.repeatsMonthlyOnDay', { day: repeatingEntity.monthDayToRepeat });
      case RepeatingEntityTypeEnum.YearRepeatingEntity:
        return t('taskForm.repeatsYearlyOnDay', { day: repeatingEntity.yearDayToRepeat });
    }
  })();

  return (
    <div className="p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg" style={{ marginTop: 12 }}>
      <Space direction="vertical" size="middle" style={{ width: '100%' }}>
        <div>
          <Typography.Text strong>{t('tasks.recurringSchedule')}</Typography.Text>
          <div style={{ marginTop: 8 }}>{repeatDescription}</div>
        </div>

        <Divider style={{ margin: '8px 0' }} />

        <Space direction="vertical" size="small" style={{ width: '100%' }}>
          <Typography.Text type="secondary" style={{ fontSize: 13 }}>
            {t('tasks.scheduleCreated')}: {dayjs(scheduleEntity.scheduleCreated).format('MMM D, YYYY HH:mm')}
          </Typography.Text>

          {scheduleEntity.endsOn ? (
            <Typography.Text type="warning" style={{ fontSize: 13 }}>
              <strong>
                {t('tasks.endsOn')}: {dayjs(scheduleEntity.endsOn).format('MMM D, YYYY')}
              </strong>
            </Typography.Text>
          ) : (
            <Typography.Text type="secondary" style={{ fontSize: 13 }}>
              {t('tasks.recurringIndefinitely')}
            </Typography.Text>
          )}
        </Space>
      </Space>
    </div>
  );
};

export default ScheduleReadOnlyInfo;
