import type { FC } from 'react';
import { Form, Checkbox, Select, InputNumber, DatePicker, theme } from 'antd';
import { useTranslation } from 'react-i18next';
import { RepeatingEntityTypeEnum } from '../../api/types';
import { getRepeatTypes, getDaysOfWeek } from './constants';

const ScheduleFormSection: FC = () => {
  const { t } = useTranslation();
  const { token } = theme.useToken();

  const addSchedule = Form.useWatch('addSchedule');
  const scheduleType = Form.useWatch('scheduleType');

  const repeatTypes = getRepeatTypes(t);
  const daysOfWeek = getDaysOfWeek(t);

  return (
    <div style={{ marginTop: 8 }}>
      <Form.Item name="addSchedule" valuePropName="checked" initialValue={false} style={{ marginBottom: 8 }}>
        <Checkbox>{t('taskForm.repeat')}</Checkbox>
      </Form.Item>

      {addSchedule && (
        <div
          style={{
            background: token.colorBgLayout,
            borderRadius: token.borderRadius,
            border: `1px solid ${token.colorBorderSecondary}`,
            padding: '12px 12px 4px',
          }}
        >
          <Form.Item
            name="scheduleType"
            label={t('taskForm.repeatType')}
            rules={[{ required: true, message: t('taskForm.selectRepeatType') }]}
            initialValue={RepeatingEntityTypeEnum.DayRepeatingEntity}
            style={{ marginBottom: 8 }}
          >
            <Select options={repeatTypes} placeholder={t('taskForm.selectType')} size="small" />
          </Form.Item>

          {scheduleType === RepeatingEntityTypeEnum.DayRepeatingEntity && (
            <Form.Item
              name="daysCountToRepeat"
              label={t('taskForm.repeatEveryNDays')}
              rules={[{ required: true, message: t('taskForm.required') }]}
              initialValue={1}
              style={{ marginBottom: 8 }}
            >
              <InputNumber min={1} style={{ width: '100%' }} size="small" />
            </Form.Item>
          )}

          {scheduleType === RepeatingEntityTypeEnum.WeekRepeatingEntity && (
            <Form.Item
              name="repeatsOn"
              label={t('taskForm.repeatOnDays')}
              rules={[
                { required: true, message: t('taskForm.selectAtLeastOneDay') },
                { type: 'array', min: 1, message: t('taskForm.selectAtLeastOneDay') },
              ]}
              style={{ marginBottom: 8 }}
            >
              <Checkbox.Group
                options={daysOfWeek}
                style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}
              />
            </Form.Item>
          )}

          {scheduleType === RepeatingEntityTypeEnum.MonthRepeatingEntity && (
            <Form.Item
              name="monthDayToRepeat"
              label={t('taskForm.dayOfMonth')}
              rules={[
                { required: true, message: t('taskForm.required') },
                { type: 'number', min: 1, max: 31, message: t('taskForm.between1And31') },
              ]}
              initialValue={1}
              style={{ marginBottom: 8 }}
            >
              <InputNumber min={1} max={31} style={{ width: '100%' }} size="small" />
            </Form.Item>
          )}

          {scheduleType === RepeatingEntityTypeEnum.YearRepeatingEntity && (
            <Form.Item
              name="yearDayToRepeat"
              label={t('taskForm.dayOfYear')}
              rules={[
                { required: true, message: t('taskForm.required') },
                { type: 'number', min: 1, max: 366, message: t('taskForm.between1And366') },
              ]}
              initialValue={1}
              style={{ marginBottom: 8 }}
            >
              <InputNumber min={1} max={366} style={{ width: '100%' }} size="small" />
            </Form.Item>
          )}

          <Form.Item name="endsOnMaxDate" label={t('taskForm.endsOnDate')} style={{ marginBottom: 8 }}>
            <DatePicker format="YYYY-MM-DD" style={{ width: '100%' }} size="small" />
          </Form.Item>

          <Form.Item name="endsOnMaxOccurrences" label={t('taskForm.endsAfterOccurrences')} style={{ marginBottom: 8 }}>
            <InputNumber
              min={1}
              placeholder={t('taskForm.noLimitPlaceholder')}
              style={{ width: '100%' }}
              size="small"
            />
          </Form.Item>
        </div>
      )}
    </div>
  );
};

export default ScheduleFormSection;
