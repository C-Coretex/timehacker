// src/components/TaskFormModal.tsx
import React, { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker } from 'antd';
import moment from 'moment';
import type { FixedTaskFormData } from '../api/types';

interface TaskFormModalProps {
    open: boolean;
    onCancel: () => void;
    onSave: (data: FixedTaskFormData, id?: string) => void;
    initialData?: FixedTaskFormData & { id: string };
}

const TaskFormModal: FC<TaskFormModalProps> = ({ open, onCancel, onSave, initialData }) => {
    const [form] = Form.useForm();

    useEffect(() => {
        if (initialData) {
            form.setFieldsValue({
                name: initialData.name,
                description: initialData.description,
                priority: initialData.priority,
                startTimestamp: initialData.startTimestamp ? moment(initialData.startTimestamp) : null,
                endTimestamp: initialData.endTimestamp ? moment(initialData.endTimestamp) : null,
            });
        } else {
            form.resetFields();
        }
    }, [initialData, form]);

    const handleFinish = (values: FixedTaskFormData) => {
        onSave(values, initialData?.id);
    };

    return (
        <Modal
            open={open}
            forceRender
            destroyOnHidden
            maskClosable={false}
            title={initialData ? 'Edit Task' : 'Add Task'}
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
                    <InputNumber min={1} max={10} />
                </Form.Item>
                <Form.Item
                    name="startTimestamp"
                    label="Start Time"
                    rules={[{ required: true, message: 'Please select start time' }]}
                >
                    <DatePicker showTime format="YYYY-MM-DD HH:mm" />
                </Form.Item>
                <Form.Item
                    name="endTimestamp"
                    label="End Time"
                    rules={[{ required: true, message: 'Please select end time' }]}
                >
                    <DatePicker showTime format="YYYY-MM-DD HH:mm" />
                </Form.Item>
            </Form>
        </Modal>
    );
};

export default TaskFormModal;