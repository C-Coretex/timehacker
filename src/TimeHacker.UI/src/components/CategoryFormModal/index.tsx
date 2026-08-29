import { useEffect, useState } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, TimePicker, ColorPicker, Button, Row, Col, Alert, Calendar } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

import type { CategoryFormData } from '../../api/types';
import { argbToHex, hexToArgb } from '../../utils/colorArgb';
import { buildSchedulePayload } from '../../utils/buildSchedulePayload';
import { useIsMobile } from '../../hooks/useIsMobile';
import { ScheduleFormSection } from '../UnifiedTaskFormModal/ScheduleFormSection';
import { ScheduleReadOnlyInfo } from '../UnifiedTaskFormModal/ScheduleReadOnlyInfo';
import type { CategoryFormModalProps } from './types';

const DEFAULT_COLOR_HEX = '#1890ff';

export const CategoryFormModal: FC<CategoryFormModalProps> = ({
  open,
  onCancel,
  onSave,
  initialData,
  defaultDate,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const { isMobile } = useIsMobile();

  const isEdit = !!initialData;
  // The day the category lands on. Held outside the Form so the inline Calendar drives it directly,
  // matching how UnifiedTaskFormModal anchors a fixed task.
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());

  useEffect(() => {
    if (!open) return;

    if (initialData) {
      form.setFieldsValue({
        name: initialData.name,
        description: initialData.description ?? '',
        color: argbToHex(initialData.color),
        startTime: initialData.startTime,
        endTime: initialData.endTime,
      });
      setSelectedDate(initialData.date);
    } else {
      form.resetFields();
      setSelectedDate(defaultDate ? dayjs(defaultDate) : dayjs());
    }
  }, [initialData, open, form, defaultDate]);

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
      date: selectedDate,
      startTime: values.startTime as Dayjs,
      endTime: values.endTime as Dayjs,
    };

    // Schedules are attached at creation only — matching the task modal, where editing shows the
    // recurrence read-only instead.
    const schedule = !isEdit ? buildSchedulePayload(values) : undefined;
    onSave(data, initialData?.id, schedule);
  };

  return (
    <Modal
      open={open}
      forceRender
      destroyOnHidden
      title={isEdit ? t('categoryForm.editCategory') : t('categoryForm.addCategory')}
      width={isMobile ? '100%' : 720}
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
        <Row gutter={24}>
          <Col span={isMobile ? 24 : 14}>
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

            <Row gutter={12}>
              <Col span={12}>
                <Form.Item
                  name="startTime"
                  label={t('categoryForm.startTime')}
                  rules={[{ required: true, message: t('categoryForm.required') }]}
                >
                  <TimePicker format="HH:mm" style={{ width: '100%' }} />
                </Form.Item>
              </Col>
              <Col span={12}>
                <Form.Item
                  name="endTime"
                  label={t('categoryForm.endTime')}
                  dependencies={['startTime']}
                  rules={[
                    { required: true, message: t('categoryForm.required') },
                    ({ getFieldValue }) => ({
                      validator(_, value: Dayjs | undefined) {
                        const start = getFieldValue('startTime') as Dayjs | undefined;
                        if (!value || !start || value.isAfter(start)) return Promise.resolve();
                        return Promise.reject(new Error(t('categoryForm.endAfterStart')));
                      },
                    }),
                  ]}
                >
                  <TimePicker format="HH:mm" style={{ width: '100%' }} />
                </Form.Item>
              </Col>
            </Row>

            <Form.Item
              name="color"
              label={t('categoryForm.color')}
              initialValue={DEFAULT_COLOR_HEX}
              getValueFromEvent={(color: { toHexString: () => string }) => color.toHexString()}
            >
              <ColorPicker disabledAlpha showText />
            </Form.Item>
          </Col>

          <Col span={isMobile ? 24 : 10}>
            <div style={{ marginBottom: 16 }}>
              <Calendar
                fullscreen={false}
                value={selectedDate}
                onSelect={(date) => setSelectedDate(date)}
              />
            </div>

            {isEdit ? (
              initialData?.scheduleEntity && (
                <ScheduleReadOnlyInfo scheduleEntity={initialData.scheduleEntity} />
              )
            ) : (
              <>
                <Alert
                  type="info"
                  showIcon
                  title={t('categoryForm.scheduleHint')}
                  style={{ marginBottom: 8 }}
                />
                <ScheduleFormSection anchorDate={selectedDate} />
              </>
            )}
          </Col>
        </Row>
      </Form>
    </Modal>
  );
};
