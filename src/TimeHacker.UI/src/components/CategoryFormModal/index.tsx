import { useEffect } from 'react';
import type { FC } from 'react';
import { ColorPicker, Form, Input } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

import { categoryColorPresets } from '../../theme/palette';
import { buildSchedulePayload } from '../../utils/buildSchedulePayload';
import { argbToHex, hexToArgb } from '../../utils/colorArgb';
import { ResponsiveFormShell } from '../ResponsiveFormShell';
import { FirstWindowFields } from './FirstWindowFields';
import type { CategoryFormModalProps } from './types';
import './styles.css';

interface CategoryFormValues extends Record<string, unknown> {
  name: string;
  description?: string;
  color: string;
  withFirstWindow?: boolean;
  date?: Dayjs;
  startTime?: Dayjs;
  endTime?: Dayjs;
}

const [DEFAULT_COLOR] = categoryColorPresets;

/**
 * A category is a label (name, colour, note). Creating one may also place its first time window; every
 * other window is managed from the category's row through CategoryScheduleFormModal.
 */
export const CategoryFormModal: FC<CategoryFormModalProps> = ({ open, onCancel, onSave, initialData }) => {
  const [form] = Form.useForm<CategoryFormValues>();
  const { t } = useTranslation();
  const isEdit = !!initialData;

  useEffect(() => {
    if (!open) return;
    form.resetFields();
    form.setFieldsValue(
      initialData
        ? { name: initialData.name, description: initialData.description ?? '', color: argbToHex(initialData.color) }
        : { color: DEFAULT_COLOR, date: dayjs() }
    );
  }, [initialData, open, form]);

  const handleFinish = (values: CategoryFormValues) => {
    const data = { name: values.name, description: values.description ?? '', color: hexToArgb(values.color) };
    const firstWindow =
      !isEdit && values.withFirstWindow && values.date && values.startTime && values.endTime
        ? {
            schedule: { description: '', date: values.date, startTime: values.startTime, endTime: values.endTime },
            recurrence: buildSchedulePayload(values),
          }
        : undefined;
    onSave(data, initialData?.id, firstWindow);
  };

  return (
    <ResponsiveFormShell
      open={open}
      onCancel={onCancel}
      title={isEdit ? t('categoryForm.editCategory') : t('categoryForm.addCategory')}
      submitLabel={isEdit ? t('categoryForm.update') : t('categoryForm.addCategory')}
      isEdit={isEdit}
      width={isEdit ? 480 : 540}
      onSubmit={() => form.submit()}
    >
      <Form form={form} layout="vertical" requiredMark={false} onFinish={handleFinish}>
        {!isEdit && <p className="th-form-hint">{t('categoryForm.createHint')}</p>}

        <div className="th-category-form__identity">
          <Form.Item
            name="name"
            label={t('categoryForm.name')}
            rules={[{ required: true, message: t('categoryForm.nameRequired') }]}
          >
            <Input placeholder={t('categoryForm.namePlaceholder')} />
          </Form.Item>
          <Form.Item
            name="color"
            label={t('categoryForm.color')}
            initialValue={DEFAULT_COLOR}
            getValueFromEvent={(color: { toHexString: () => string }) => color.toHexString()}
          >
            <ColorPicker disabledAlpha presets={[{ label: t('categoryForm.suggestedColors'), colors: [...categoryColorPresets] }]} />
          </Form.Item>
        </div>

        <Form.Item name="description" label={t('categoryForm.description')}>
          <Input.TextArea rows={2} placeholder={t('categoryForm.descriptionPlaceholder')} />
        </Form.Item>

        {!isEdit && <FirstWindowFields />}
      </Form>
    </ResponsiveFormShell>
  );
};
