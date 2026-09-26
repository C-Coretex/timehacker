import type { CSSProperties, FC } from 'react';
import { Form } from 'antd';
import { useTranslation } from 'react-i18next';
import { DurationField } from 'components/DurationField';

// Wide enough for "Minimum" / "Оптимально" beside the clock.
const LABEL_WIDTH = { '--th-clock-label-width': '92px' } as CSSProperties;

/**
 * How long a dynamic task may take, as H:MM clocks. The scheduler fits it into category windows by these
 * bounds, which is why a dynamic task has durations where a fixed one has a date and time.
 */
export const DurationFields: FC = () => {
  const { t } = useTranslation();

  return (
    <div className="th-task-durations">
      <div className="th-task-durations__title">{t('taskForm.duration')}</div>
      <div className="th-clock-fields" style={LABEL_WIDTH}>
        <Form.Item
          name="minMinutes"
          label={t('dynamicTaskForm.minDuration')}
          layout="horizontal"
          colon={false}
          rules={[{ required: true, message: t('dynamicTaskForm.minDurationRequired') }]}
        >
          <DurationField />
        </Form.Item>
        <Form.Item
          name="maxMinutes"
          label={t('dynamicTaskForm.maxDuration')}
          layout="horizontal"
          colon={false}
          dependencies={['minMinutes']}
          rules={[
            { required: true, message: t('dynamicTaskForm.maxDurationRequired') },
            ({ getFieldValue }) => ({
              validator: (_, max: number | undefined) => {
                const min = getFieldValue('minMinutes') as number | undefined;
                return max == null || min == null || max > min
                  ? Promise.resolve()
                  : Promise.reject(new Error(t('dynamicTaskForm.maxAfterMin')));
              },
            }),
          ]}
        >
          <DurationField />
        </Form.Item>
        <Form.Item
          name="optimalMinutes"
          label={t('dynamicTaskForm.optimalDuration')}
          layout="horizontal"
          colon={false}
          extra={t('dynamicTaskForm.optional')}
        >
          <DurationField optional />
        </Form.Item>
      </div>
    </div>
  );
};
