import type { FC } from 'react';
import { DatePicker, Form, InputNumber } from 'antd';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { PillGroup } from 'components/PillGroup';
import { getEndsOptions } from './constants';
import type { EndsMode } from './constants';

/**
 * When a recurrence stops: never, on a date, or after N times. The mode is UI state kept in the form store
 * (unregistered, so it is never submitted); switching it clears both limits, so only the chosen one is sent.
 */
export const EndsFields: FC<{ anchor: Dayjs }> = ({ anchor }) => {
  const { t } = useTranslation();
  const form = Form.useFormInstance();
  const mode = Form.useWatch<EndsMode | undefined>('endsMode', { form, preserve: true }) ?? 'never';
  const count = Form.useWatch<number | undefined>('endsOnMaxOccurrences', form) ?? 1;

  const switchMode = (next: EndsMode) =>
    form.setFieldsValue({ endsMode: next, endsOnMaxDate: undefined, endsOnMaxOccurrences: undefined });

  return (
    <Form.Item label={t('taskForm.ends')}>
      <div className="th-repeat-ends">
        <PillGroup<EndsMode> aria-label={t('taskForm.ends')} options={getEndsOptions(t)} value={mode} onChange={switchMode} />
        {mode === 'date' && (
          <Form.Item name="endsOnMaxDate" noStyle rules={[{ required: true, message: t('taskForm.endDateRequired') }]}>
            <DatePicker format="D MMM YYYY" disabledDate={(day) => !day.isAfter(anchor, 'day')} />
          </Form.Item>
        )}
        {mode === 'count' && (
          <span className="th-repeat-inline">
            <Form.Item name="endsOnMaxOccurrences" initialValue={5} noStyle>
              <InputNumber mode="spinner" min={1} max={999} precision={0} className="th-repeat-count" />
            </Form.Item>
            <span>{t('taskForm.timesUnit', { count })}</span>
          </span>
        )}
      </div>
    </Form.Item>
  );
};
