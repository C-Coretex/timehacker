import type { FC } from 'react';
import { Form } from 'antd';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { DigitalTimeField } from 'components/DigitalTimeField';

const minutesOfDay = (time: Dayjs) => time.hour() * 60 + time.minute();

/** "From / To" clocks bound to `startTime` / `endTime`, in 5-minute steps; To must come after From. */
export const TimeRangeFields: FC = () => {
  const { t } = useTranslation();

  return (
    <div className="th-clock-fields">
      <Form.Item
        name="startTime"
        label={t('taskForm.from')}
        layout="horizontal"
        colon={false}
        rules={[{ required: true, message: t('taskForm.startTimeRequired') }]}
      >
        <DigitalTimeField />
      </Form.Item>
      <Form.Item
        name="endTime"
        label={t('taskForm.to')}
        layout="horizontal"
        colon={false}
        dependencies={['startTime']}
        rules={[
          { required: true, message: t('taskForm.endTimeRequired') },
          ({ getFieldValue }) => ({
            validator: (_, end: Dayjs | undefined) => {
              const start = getFieldValue('startTime') as Dayjs | undefined;
              return !end || !start || minutesOfDay(end) > minutesOfDay(start)
                ? Promise.resolve()
                : Promise.reject(new Error(t('taskForm.endAfterStart')));
            },
          }),
        ]}
      >
        <DigitalTimeField />
      </Form.Item>
    </div>
  );
};
