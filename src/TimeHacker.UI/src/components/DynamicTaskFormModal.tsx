import { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, InputNumber } from 'antd';
import { useTranslation } from 'react-i18next';
import type { DynamicTaskReturnModel, InputDynamicTask } from '../api/types';

export interface DynamicTaskFormData {
  name: string;
  description: string;
  priority: number;
  minMinutes: number;
  maxMinutes: number;
  optimalMinutes: number | null;
}

function minutesToTimeSpan(minutes: number): string {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${pad(h)}:${pad(m)}:00`;
}

function timeSpanToMinutes(value: string): number {
  if (!value) return 0;
  const parts = value.split(/[.:]/).map(Number);
  if (parts.length >= 3) {
    const [h, m] = parts;
    return (h ?? 0) * 60 + (m ?? 0);
  }
  return 0;
}

interface DynamicTaskFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSave: (data: InputDynamicTask, id?: string) => void;
  initialData?: DynamicTaskReturnModel | null;
}

const DynamicTaskFormModal: FC<DynamicTaskFormModalProps> = ({
  open,
  onCancel,
  onSave,
  initialData,
}) => {
  const [form] = Form.useForm<DynamicTaskFormData>();
  const { t } = useTranslation();

  useEffect(() => {
    if (initialData) {
      form.setFieldsValue({
        name: initialData.name,
        description: initialData.description ?? '',
        priority: initialData.priority,
        minMinutes: timeSpanToMinutes(initialData.minTimeToFinish),
        maxMinutes: timeSpanToMinutes(initialData.maxTimeToFinish),
        optimalMinutes: initialData.optimalTimeToFinish
          ? timeSpanToMinutes(initialData.optimalTimeToFinish)
          : null,
      });
    } else {
      form.resetFields();
    }
  }, [initialData, open, form]);

  const handleFinish = (values: DynamicTaskFormData) => {
    const payload: InputDynamicTask = {
      name: values.name,
      description: values.description || undefined,
      priority: values.priority,
      minTimeToFinish: minutesToTimeSpan(values.minMinutes),
      maxTimeToFinish: minutesToTimeSpan(values.maxMinutes),
      optimalTimeToFinish:
        values.optimalMinutes != null && values.optimalMinutes > 0
          ? minutesToTimeSpan(values.optimalMinutes)
          : undefined,
    };
    onSave(payload, initialData?.id);
  };

  return (
    <Modal
      open={open}
      forceRender
      destroyOnClose
      maskClosable={false}
      title={initialData ? t('dynamicTaskForm.editTask') : t('dynamicTaskForm.addTask')}
      okText={initialData ? t('dynamicTaskForm.update') : t('dynamicTaskForm.create')}
      onCancel={onCancel}
      onOk={() => form.submit()}
    >
      <Form form={form} onFinish={handleFinish} layout="vertical">
        <Form.Item
          name="name"
          label={t('dynamicTaskForm.taskName')}
          rules={[{ required: true, message: t('dynamicTaskForm.taskNameRequired') }]}
        >
          <Input />
        </Form.Item>
        <Form.Item name="description" label={t('dynamicTaskForm.description')}>
          <Input.TextArea />
        </Form.Item>
        <Form.Item
          name="priority"
          label={t('dynamicTaskForm.priority')}
          rules={[{ required: true, message: t('dynamicTaskForm.priorityRequired') }]}
        >
          <InputNumber min={0} max={255} />
        </Form.Item>
        <Form.Item
          name="minMinutes"
          label={t('dynamicTaskForm.minDuration')}
          rules={[{ required: true, message: t('dynamicTaskForm.minDurationRequired') }]}
        >
          <InputNumber min={1} />
        </Form.Item>
        <Form.Item
          name="maxMinutes"
          label={t('dynamicTaskForm.maxDuration')}
          rules={[{ required: true, message: t('dynamicTaskForm.maxDurationRequired') }]}
        >
          <InputNumber min={1} />
        </Form.Item>
        <Form.Item name="optimalMinutes" label={t('dynamicTaskForm.optimalDuration')}>
          <InputNumber min={0} placeholder={t('dynamicTaskForm.optional')} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default DynamicTaskFormModal;
