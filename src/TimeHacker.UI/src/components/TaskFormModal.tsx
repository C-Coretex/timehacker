// src/components/TaskFormModal.tsx
import { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, Checkbox, Select, Collapse, Alert } from 'antd';
import dayjs from 'dayjs';
import type {
    FixedTaskFormData,
    InputRepeatingEntityType,
    RepeatingEntityTypeName,
    EndsOnModel,
} from '../api/types';

export type ScheduleFormPayload = {
    repeatingEntityType: InputRepeatingEntityType;
    endsOnModel?: EndsOnModel | null;
};

interface TaskFormModalProps {
    open: boolean;
    onCancel: () => void;
    onSave: (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => void;
    initialData?: FixedTaskFormData & { id: string };
}

const REPEAT_TYPES: { value: RepeatingEntityTypeName; label: string }[] = [
    { value: 'DayRepeatingEntity', label: 'Every N days' },
    { value: 'WeekRepeatingEntity', label: 'Weekly (specific days)' },
    { value: 'MonthRepeatingEntity', label: 'Monthly (day of month)' },
    { value: 'YearRepeatingEntity', label: 'Yearly (day of year)' },
];

const DAYS_OF_WEEK = [
    { value: 1, label: 'Mon' },
    { value: 2, label: 'Tue' },
    { value: 3, label: 'Wed' },
    { value: 4, label: 'Thu' },
    { value: 5, label: 'Fri' },
    { value: 6, label: 'Sat' },
    { value: 7, label: 'Sun' },
];

const TaskFormModal: FC<TaskFormModalProps> = ({ open, onCancel, onSave, initialData }) => {
    const [form] = Form.useForm();
    const addSchedule = Form.useWatch('addSchedule', form);
    const scheduleType = Form.useWatch('scheduleType', form);
    const isCreate = !initialData;

    useEffect(() => {
        if (initialData) {
            form.setFieldsValue({
                name: initialData.name,
                description: initialData.description,
                priority: initialData.priority,
                startTimestamp: initialData.startTimestamp ? dayjs(initialData.startTimestamp) : null,
                endTimestamp: initialData.endTimestamp ? dayjs(initialData.endTimestamp) : null,
            });
        } else {
            form.resetFields();
        }
    }, [initialData, form]);

    const buildSchedulePayload = (): ScheduleFormPayload | undefined => {
        if (!addSchedule || !scheduleType) return undefined;
        const values = form.getFieldsValue();
        let repeatingEntityType: InputRepeatingEntityType;
        switch (scheduleType) {
            case 'DayRepeatingEntity':
                repeatingEntityType = {
                    entityType: 'DayRepeatingEntity',
                    daysCountToRepeat: values.daysCountToRepeat ?? 1,
                };
                break;
            case 'WeekRepeatingEntity':
                repeatingEntityType = {
                    entityType: 'WeekRepeatingEntity',
                    repeatsOn: values.repeatsOn ?? [],
                };
                break;
            case 'MonthRepeatingEntity':
                repeatingEntityType = {
                    entityType: 'MonthRepeatingEntity',
                    monthDayToRepeat: values.monthDayToRepeat ?? 1,
                };
                break;
            case 'YearRepeatingEntity':
                repeatingEntityType = {
                    entityType: 'YearRepeatingEntity',
                    yearDayToRepeat: values.yearDayToRepeat ?? 1,
                };
                break;
            default:
                return undefined;
        }
        const endsOnModel: EndsOnModel | undefined =
            values.endsOnMaxDate || values.endsOnMaxOccurrences != null
                ? {
                      maxDate: values.endsOnMaxDate
                          ? dayjs(values.endsOnMaxDate).format('YYYY-MM-DD')
                          : undefined,
                      maxOccurrences:
                          values.endsOnMaxOccurrences != null && values.endsOnMaxOccurrences > 0
                              ? values.endsOnMaxOccurrences
                              : undefined,
                  }
                : undefined;
        return { repeatingEntityType, endsOnModel: endsOnModel ?? null };
    };

    const handleFinish = (values: Record<string, unknown>) => {
        const taskData: FixedTaskFormData = {
            name: values.name as string,
            description: values.description as string,
            priority: values.priority as number,
            startTimestamp: values.startTimestamp as FixedTaskFormData['startTimestamp'],
            endTimestamp: values.endTimestamp as FixedTaskFormData['endTimestamp'],
        };
        const schedule = isCreate ? buildSchedulePayload() : undefined;
        onSave(taskData, initialData?.id, schedule);
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

                {!isCreate && (
                    <Alert
                        type="info"
                        message="Schedule Information"
                        description="If this task has a recurring schedule, it cannot be viewed or modified here. Schedules are set only during task creation."
                        showIcon
                        style={{ marginBottom: 16 }}
                    />
                )}

                {isCreate && (
                    <>
                        <Form.Item name="addSchedule" valuePropName="checked" initialValue={false}>
                            <Checkbox>Add schedule (repeating)</Checkbox>
                        </Form.Item>
                        {addSchedule && (
                            <Collapse defaultActiveKey={['schedule']}>
                                <Collapse.Panel header="Schedule" key="schedule">
                                    <Form.Item
                                        name="scheduleType"
                                        label="Repeat type"
                                        rules={[
                                            {
                                                required: true,
                                                message: 'Select repeat type',
                                            },
                                        ]}
                                        initialValue="DayRepeatingEntity"
                                    >
                                        <Select
                                            options={REPEAT_TYPES}
                                            placeholder="Select type"
                                        />
                                    </Form.Item>

                                    {scheduleType === 'DayRepeatingEntity' && (
                                        <Form.Item
                                            name="daysCountToRepeat"
                                            label="Repeat every N days"
                                            rules={[
                                                {
                                                    required: true,
                                                    message: 'Required',
                                                },
                                            ]}
                                            initialValue={1}
                                        >
                                            <InputNumber min={1} style={{ width: '100%' }} />
                                        </Form.Item>
                                    )}

                                    {scheduleType === 'WeekRepeatingEntity' && (
                                        <Form.Item
                                            name="repeatsOn"
                                            label="Repeat on days"
                                            rules={[
                                                {
                                                    required: true,
                                                    message: 'Select at least one day',
                                                },
                                                {
                                                    type: 'array',
                                                    min: 1,
                                                    message: 'Select at least one day',
                                                },
                                            ]}
                                        >
                                            <Checkbox.Group
                                                options={DAYS_OF_WEEK}
                                                style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}
                                            />
                                        </Form.Item>
                                    )}

                                    {scheduleType === 'MonthRepeatingEntity' && (
                                        <Form.Item
                                            name="monthDayToRepeat"
                                            label="Day of month (1–31)"
                                            rules={[
                                                { required: true, message: 'Required' },
                                                {
                                                    type: 'number',
                                                    min: 1,
                                                    max: 31,
                                                    message: 'Between 1 and 31',
                                                },
                                            ]}
                                            initialValue={1}
                                        >
                                            <InputNumber
                                                min={1}
                                                max={31}
                                                style={{ width: '100%' }}
                                            />
                                        </Form.Item>
                                    )}

                                    {scheduleType === 'YearRepeatingEntity' && (
                                        <Form.Item
                                            name="yearDayToRepeat"
                                            label="Day of year (1–366)"
                                            rules={[
                                                { required: true, message: 'Required' },
                                                {
                                                    type: 'number',
                                                    min: 1,
                                                    max: 366,
                                                    message: 'Between 1 and 366',
                                                },
                                            ]}
                                            initialValue={1}
                                        >
                                            <InputNumber
                                                min={1}
                                                max={366}
                                                style={{ width: '100%' }}
                                            />
                                        </Form.Item>
                                    )}

                                    <Form.Item name="endsOnMaxDate" label="Ends on date (optional)">
                                        <DatePicker format="YYYY-MM-DD" style={{ width: '100%' }} />
                                    </Form.Item>
                                    <Form.Item
                                        name="endsOnMaxOccurrences"
                                        label="End after N occurrences (optional)"
                                    >
                                        <InputNumber
                                            min={1}
                                            placeholder="Leave empty for no limit"
                                            style={{ width: '100%' }}
                                        />
                                    </Form.Item>
                                </Collapse.Panel>
                            </Collapse>
                        )}
                    </>
                )}
            </Form>
        </Modal>
    );
};

export default TaskFormModal;
