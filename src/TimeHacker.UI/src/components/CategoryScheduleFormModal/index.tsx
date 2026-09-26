import { useEffect } from 'react';
import type { FC } from 'react';
import { Form, Input } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

import { buildSchedulePayload } from '../../utils/buildSchedulePayload';
import { ResponsiveFormShell } from '../ResponsiveFormShell';
import { WhenFields } from '../WhenFields';
import type { CategoryScheduleFormModalProps } from './types';

interface ScheduleFormValues extends Record<string, unknown> {
  description?: string;
  date: Dayjs;
  startTime: Dayjs;
  endTime: Dayjs;
}

/** One dated time window of a category: its day and hours, a recurrence on create, and an optional note. */
export const CategoryScheduleFormModal: FC<CategoryScheduleFormModalProps> = ({
  open,
  onCancel,
  onSave,
  categoryName,
  initialData,
  defaultDate,
}) => {
  const [form] = Form.useForm<ScheduleFormValues>();
  const { t } = useTranslation();
  const isEdit = !!initialData;

  useEffect(() => {
    if (!open) return;
    form.resetFields();
    form.setFieldsValue(
      initialData
        ? {
            description: initialData.description ?? '',
            date: initialData.date,
            startTime: initialData.startTime,
            endTime: initialData.endTime,
          }
        : { date: defaultDate ? dayjs(defaultDate) : dayjs() }
    );
  }, [initialData, open, form, defaultDate]);

  const handleFinish = (values: ScheduleFormValues) => {
    const data = {
      description: values.description ?? '',
      date: values.date,
      startTime: values.startTime,
      endTime: values.endTime,
    };
    // Recurrences are attached at creation only — editing shows the existing one read-only.
    onSave(data, initialData?.id, isEdit ? undefined : buildSchedulePayload(values));
  };

  return (
    <ResponsiveFormShell
      open={open}
      onCancel={onCancel}
      title={
        isEdit
          ? t('categoryScheduleForm.editSchedule', { category: categoryName })
          : t('categoryScheduleForm.addSchedule', { category: categoryName })
      }
      submitLabel={isEdit ? t('categoryScheduleForm.update') : t('categoryScheduleForm.create')}
      isEdit={isEdit}
      width={520}
      onSubmit={() => form.submit()}
    >
      <Form form={form} layout="vertical" requiredMark={false} onFinish={handleFinish}>
        {!isEdit && <p className="th-form-hint">{t('categoryScheduleForm.scheduleHint')}</p>}
        <WhenFields isEdit={isEdit} scheduleEntity={initialData?.scheduleEntity} />
        {/* Optional: the category already names this window; this only tells siblings apart. */}
        <Form.Item name="description" label={t('categoryScheduleForm.description')} className="th-form-after-card">
          <Input placeholder={t('categoryScheduleForm.descriptionPlaceholder')} />
        </Form.Item>
      </Form>
    </ResponsiveFormShell>
  );
};
