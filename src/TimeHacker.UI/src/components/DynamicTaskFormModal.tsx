import { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, InputNumber } from 'antd';
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
      title={initialData ? 'Edit Dynamic Task' : 'Add Dynamic Task'}
      okText={initialData ? 'Update' : 'Create'}
      onCancel={onCancel}
      onOk={() => form.submit()}
    >
      <Form form={form} onFinish={handleFinish} layout="vertical">
        <Form.Item
          name="name"
          label="Task Name"
          rules={[{ required: true, message: 'Please enter task name' }]}
        >
          <Input />
        </Form.Item>
        <Form.Item name="description" label="Description">
          <Input.TextArea />
        </Form.Item>
        <Form.Item
          name="priority"
          label="Priority"
          rules={[{ required: true, message: 'Please enter priority' }]}
        >
          <InputNumber min={0} max={255} />
        </Form.Item>
        <Form.Item
          name="minMinutes"
          label="Min duration (minutes)"
          rules={[{ required: true, message: 'Please enter min duration' }]}
        >
          <InputNumber min={1} />
        </Form.Item>
        <Form.Item
          name="maxMinutes"
          label="Max duration (minutes)"
          rules={[{ required: true, message: 'Please enter max duration' }]}
        >
          <InputNumber min={1} />
        </Form.Item>
        <Form.Item name="optimalMinutes" label="Optimal duration (minutes, optional)">
          <InputNumber min={0} placeholder="Optional" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default DynamicTaskFormModal;
