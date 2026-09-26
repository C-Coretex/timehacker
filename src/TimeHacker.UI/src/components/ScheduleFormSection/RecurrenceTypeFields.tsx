import type { FC } from 'react';
import { DatePicker, Form, InputNumber } from 'antd';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { RepeatingEntityTypeEnum } from 'api/types';
import { MonthDayGrid, WeekdayToggles, YearDayPicker } from './DayPickers';

interface RecurrenceTypeFieldsProps {
  scheduleType: RepeatingEntityTypeEnum;
  /** The day the entity occupies; each type starts from it (its weekday, its day of the month…). */
  anchor: Dayjs;
  /** "Once" dates must fall after this day — see ScheduleFormSection. */
  onceFloor: Dayjs;
}

const isoWeekday = (date: Dayjs) => (date.day() === 0 ? 7 : date.day());
const dayOfYear = (date: Dayjs) => date.diff(date.startOf('year'), 'day') + 1;

/** "Every [− 2 +] days" */
const EveryNDays: FC = () => {
  const { t } = useTranslation();
  const count = Form.useWatch<number | undefined>('daysCountToRepeat') ?? 1;

  return (
    <Form.Item label={t('taskForm.every')}>
      <span className="th-repeat-inline">
        <Form.Item name="daysCountToRepeat" initialValue={1} noStyle>
          <InputNumber mode="spinner" min={1} max={365} precision={0} className="th-repeat-count" />
        </Form.Item>
        <span>{t('taskForm.dayUnit', { count })}</span>
      </span>
    </Form.Item>
  );
};

/** The fields specific to the chosen recurrence type, each starting from the anchor day. */
export const RecurrenceTypeFields: FC<RecurrenceTypeFieldsProps> = ({ scheduleType, anchor, onceFloor }) => {
  const { t } = useTranslation();

  switch (scheduleType) {
    case RepeatingEntityTypeEnum.DayRepeatingEntity:
      return <EveryNDays />;
    case RepeatingEntityTypeEnum.WeekRepeatingEntity:
      return (
        <Form.Item
          name="repeatsOn"
          label={t('taskForm.weeklyOn')}
          initialValue={[isoWeekday(anchor)]}
          rules={[{ required: true, type: 'array', min: 1, message: t('taskForm.selectAtLeastOneDay') }]}
        >
          <WeekdayToggles />
        </Form.Item>
      );
    case RepeatingEntityTypeEnum.MonthRepeatingEntity:
      return (
        <Form.Item name="monthDayToRepeat" label={t('taskForm.monthlyOn')} initialValue={anchor.date()}>
          <MonthDayGrid />
        </Form.Item>
      );
    case RepeatingEntityTypeEnum.YearRepeatingEntity:
      return (
        <Form.Item name="yearDayToRepeat" label={t('taskForm.yearlyOn')} initialValue={dayOfYear(anchor)}>
          <YearDayPicker year={anchor.year()} />
        </Form.Item>
      );
    case RepeatingEntityTypeEnum.OnceRepeatingEntity:
      return (
        <Form.Item
          name="onceDates"
          label={t('taskForm.specificDates')}
          rules={[
            { required: true, type: 'array', min: 1, message: t('taskForm.selectAtLeastOneDate') },
            // disabledDate stops new picks, but the anchor can move after dates were chosen.
            {
              validator: (_, value: Dayjs[] | undefined) =>
                value?.some((date) => !date.isAfter(onceFloor, 'day'))
                  ? Promise.reject(new Error(t('taskForm.datesMustBeAfter', { date: onceFloor.format('D MMM YYYY') })))
                  : Promise.resolve(),
            },
          ]}
        >
          <DatePicker
            multiple
            format="D MMM YYYY"
            style={{ width: '100%' }}
            placeholder={t('taskForm.selectDates')}
            disabledDate={(current) => !current.isAfter(onceFloor, 'day')}
          />
        </Form.Item>
      );
  }
};
