import { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, ColorPicker, Button } from 'antd';
import { useTranslation } from 'react-i18next';

import type { CategoryFormData } from '../../api/types';
import { argbToHex, hexToArgb } from '../../utils/colorArgb';
import { useIsMobile } from '../../hooks/useIsMobile';
import type { CategoryFormModalProps } from './types';

const DEFAULT_COLOR_HEX = '#1890ff';

/**
 * A category is just a label. Its dated time windows are managed separately, through
 * CategoryScheduleFormModal on the expanded row.
 */
export const CategoryFormModal: FC<CategoryFormModalProps> = ({
  open,
  onCancel,
  onSave,
  initialData,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const { isMobile } = useIsMobile();

  const isEdit = !!initialData;

  useEffect(() => {
    if (!open) return;

    if (initialData) {
      form.setFieldsValue({
        name: initialData.name,
        description: initialData.description ?? '',
        color: argbToHex(initialData.color),
      });
    } else {
      form.resetFields();
    }
  }, [initialData, open, form]);

  const handleFinish = (values: Record<string, unknown>) => {
    const rawColor = values.color as { toHexString?: () => string } | string | undefined;
    const hex =
      typeof rawColor === 'string'
        ? rawColor
        : (rawColor?.toHexString?.() ?? DEFAULT_COLOR_HEX);

    const data: CategoryFormData = {
      name: values.name as string,
      description: (values.description as string) ?? '',
      color: hexToArgb(hex),
    };

    onSave(data, initialData?.id);
  };

  return (
    <Modal
      open={open}
      forceRender
      destroyOnHidden
      title={isEdit ? t('categoryForm.editCategory') : t('categoryForm.addCategory')}
      width={isMobile ? '100%' : 480}
      onCancel={onCancel}
      footer={
        <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
          <Button type="primary" block size="large" onClick={() => form.submit()}>
            {isEdit ? t('categoryForm.update') : t('categoryForm.create')}
          </Button>
          <Button type="text" block size="small" onClick={onCancel}>
            {t('categoryForm.cancel')}
          </Button>
        </div>
      }
    >
      <Form form={form} onFinish={handleFinish} layout="vertical">
        <Form.Item
          name="name"
          label={t('categoryForm.name')}
          rules={[{ required: true, message: t('categoryForm.nameRequired') }]}
        >
          <Input placeholder={t('categoryForm.namePlaceholder')} />
        </Form.Item>

        <Form.Item name="description" label={t('categoryForm.description')}>
          <Input.TextArea rows={3} placeholder={t('categoryForm.descriptionPlaceholder')} />
        </Form.Item>

        <Form.Item
          name="color"
          label={t('categoryForm.color')}
          initialValue={DEFAULT_COLOR_HEX}
          getValueFromEvent={(color: { toHexString: () => string }) => color.toHexString()}
        >
          <ColorPicker disabledAlpha showText />
        </Form.Item>
      </Form>
    </Modal>
  );
};
