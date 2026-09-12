import { useEffect, useState } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, TimePicker, Button, Row, Col, Alert, Calendar } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';

import type { CategoryScheduleFormData } from '../../api/types';
import { buildSchedulePayload } from '../../utils/buildSchedulePayload';
import { useIsMobile } from '../../hooks/useIsMobile';
import { ScheduleFormSection } from '../UnifiedTaskFormModal/ScheduleFormSection';
import { ScheduleReadOnlyInfo } from '../UnifiedTaskFormModal/ScheduleReadOnlyInfo';
import type { CategoryScheduleFormModalProps } from './types';

/**
 * One dated time window of a category. Mirrors UnifiedTaskFormModal: the date lives outside the Form so
 * the inline Calendar drives it, and the recurrence is attachable on create only.
 */
export const CategoryScheduleFormModal: FC<CategoryScheduleFormModalProps> = ({
  open,
  onCancel,
  onSave,
  categoryName,
  initialData,
  defaultDate,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const { isMobile } = useIsMobile();

  const isEdit = !!initialData;
  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs());

  useEffect(() => {
    if (!open) return;

    if (initialData) {
      form.setFieldsValue({
        description: initialData.description ?? '',
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
    const data: CategoryScheduleFormData = {
      description: (values.description as string) ?? '',
      date: selectedDate,
      startTime: values.startTime as Dayjs,
      endTime: values.endTime as Dayjs,
    };

    // Recurrences are attached at creation only — matching the task modal, where editing shows the
    // recurrence read-only instead.
    const recurrence = !isEdit ? buildSchedulePayload(values) : undefined;
    onSave(data, initialData?.id, recurrence);
  };

  return (
    <Modal
      open={open}
      forceRender
      destroyOnHidden
      title={
        isEdit
          ? t('categoryScheduleForm.editSchedule', { category: categoryName })
          : t('categoryScheduleForm.addSchedule', { category: categoryName })
      }
      width={isMobile ? '100%' : 720}
      onCancel={onCancel}
      footer={
        <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
          <Button type="primary" block size="large" onClick={() => form.submit()}>
            {isEdit ? t('categoryScheduleForm.update') : t('categoryScheduleForm.create')}
          </Button>
          <Button type="text" block size="small" onClick={onCancel}>
            {t('categoryScheduleForm.cancel')}
          </Button>
        </div>
      }
    >
      <Form form={form} onFinish={handleFinish} layout="vertical">
        <Row gutter={24}>
          <Col span={isMobile ? 24 : 14}>
            {/* Optional: the category already names this window; this only tells siblings apart. */}
            <Form.Item name="description" label={t('categoryScheduleForm.description')}>
              <Input placeholder={t('categoryScheduleForm.descriptionPlaceholder')} />
            </Form.Item>

            <Row gutter={12}>
              <Col span={12}>
                <Form.Item
                  name="startTime"
                  label={t('categoryScheduleForm.startTime')}
                  rules={[{ required: true, message: t('categoryScheduleForm.required') }]}
                >
                  <TimePicker format="HH:mm" style={{ width: '100%' }} />
                </Form.Item>
              </Col>
              <Col span={12}>
                <Form.Item
                  name="endTime"
                  label={t('categoryScheduleForm.endTime')}
                  dependencies={['startTime']}
                  rules={[
                    { required: true, message: t('categoryScheduleForm.required') },
                    ({ getFieldValue }) => ({
                      validator(_, value: Dayjs | undefined) {
                        const start = getFieldValue('startTime') as Dayjs | undefined;
                        if (!value || !start || value.isAfter(start)) return Promise.resolve();
                        return Promise.reject(new Error(t('categoryScheduleForm.endAfterStart')));
                      },
                    }),
                  ]}
                >
                  <TimePicker format="HH:mm" style={{ width: '100%' }} />
                </Form.Item>
              </Col>
            </Row>
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
                  title={t('categoryScheduleForm.scheduleHint')}
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
