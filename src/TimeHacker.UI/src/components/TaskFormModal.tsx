// src/components/TaskFormModal.tsx
import { useEffect } from 'react';
import type { FC } from 'react';
import { Modal, Form, Input, InputNumber, DatePicker, Checkbox, Select, Collapse, Descriptions } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import {
    RepeatingEntityTypeEnum,
} from '../api/types';
import type {
    FixedTaskFormData,
    InputRepeatingEntityType,
    EndsOnModel,
    ReturnRepeatingEntityModel,
} from '../api/types';

export type ScheduleFormPayload = {
    repeatingEntityType: InputRepeatingEntityType;
    endsOnModel?: EndsOnModel | null;
};

interface TaskFormModalProps {
    open: boolean;
    onCancel: () => void;
    onSave: (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => void;
    initialData?: FixedTaskFormData & { id: string; repeatingEntity?: ReturnRepeatingEntityModel | null };
}

const TaskFormModal: FC<TaskFormModalProps> = ({ open, onCancel, onSave, initialData }) => {
    const [form] = Form.useForm();
    const { t } = useTranslation();
    const addSchedule = Form.useWatch('addSchedule', form);
    const scheduleType = Form.useWatch('scheduleType', form);
    const isCreate = !initialData;

    const repeatTypes: { value: RepeatingEntityTypeEnum; label: string }[] = [
        { value: RepeatingEntityTypeEnum.DayRepeatingEntity, label: t('taskForm.everyNDays') },
        { value: RepeatingEntityTypeEnum.WeekRepeatingEntity, label: t('taskForm.weekly') },
        { value: RepeatingEntityTypeEnum.MonthRepeatingEntity, label: t('taskForm.monthly') },
        { value: RepeatingEntityTypeEnum.YearRepeatingEntity, label: t('taskForm.yearly') },
    ];

    const daysOfWeek = [
        { value: 1, label: t('taskForm.mon') },
        { value: 2, label: t('taskForm.tue') },
        { value: 3, label: t('taskForm.wed') },
        { value: 4, label: t('taskForm.thu') },
        { value: 5, label: t('taskForm.fri') },
        { value: 6, label: t('taskForm.sat') },
        { value: 7, label: t('taskForm.sun') },
    ];

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
        if (!addSchedule || scheduleType == null) return undefined;
        const values = form.getFieldsValue();
        let repeatingEntityType: InputRepeatingEntityType;
        switch (scheduleType as RepeatingEntityTypeEnum) {
            case RepeatingEntityTypeEnum.DayRepeatingEntity:
                repeatingEntityType = {
                    entityType: RepeatingEntityTypeEnum.DayRepeatingEntity,
                    daysCountToRepeat: values.daysCountToRepeat ?? 1,
                };
                break;
            case RepeatingEntityTypeEnum.WeekRepeatingEntity:
                repeatingEntityType = {
                    entityType: RepeatingEntityTypeEnum.WeekRepeatingEntity,
                    repeatsOn: values.repeatsOn ?? [],
                };
                break;
            case RepeatingEntityTypeEnum.MonthRepeatingEntity:
                repeatingEntityType = {
                    entityType: RepeatingEntityTypeEnum.MonthRepeatingEntity,
                    monthDayToRepeat: values.monthDayToRepeat ?? 1,
                };
                break;
            case RepeatingEntityTypeEnum.YearRepeatingEntity:
                repeatingEntityType = {
                    entityType: RepeatingEntityTypeEnum.YearRepeatingEntity,
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
            mask={{ closable: false }}
            title={initialData ? t('taskForm.editTask') : t('taskForm.addTask')}
            okText={initialData ? t('taskForm.update') : t('taskForm.create')}
            onCancel={onCancel}
            onOk={() => form.submit()}
        >
            <Form form={form} onFinish={handleFinish} layout="vertical">
                <Form.Item
                    name="name"
                    label={t('taskForm.taskName')}
                    rules={[{ required: true, message: t('taskForm.taskNameRequired') }]}
                >
                    <Input />
                </Form.Item>
                <Form.Item name="description" label={t('taskForm.description')}>
                    <Input.TextArea />
                </Form.Item>
                <Form.Item
                    name="priority"
                    label={t('taskForm.priority')}
                    rules={[{ required: true, message: t('taskForm.priorityRequired') }]}
                >
                    <InputNumber min={1} max={10} />
                </Form.Item>
                <Form.Item
                    name="startTimestamp"
                    label={t('taskForm.startTime')}
                    rules={[{ required: true, message: t('taskForm.startTimeRequired') }]}
                >
                    <DatePicker showTime format="YYYY-MM-DD HH:mm" />
                </Form.Item>
                <Form.Item
                    name="endTimestamp"
                    label={t('taskForm.endTime')}
                    rules={[{ required: true, message: t('taskForm.endTimeRequired') }]}
                >
                    <DatePicker showTime format="YYYY-MM-DD HH:mm" />
                </Form.Item>

                {!isCreate && initialData?.repeatingEntity && (
                    <Descriptions
                        title={t('taskForm.currentSchedule')}
                        bordered
                        size="small"
                        column={1}
                        style={{ marginBottom: 16 }}
                    >
                        <Descriptions.Item label={t('taskForm.repeatType')}>
                            {initialData.repeatingEntity.entityType === RepeatingEntityTypeEnum.DayRepeatingEntity &&
                                t('taskForm.repeatsEveryNDays', { count: initialData.repeatingEntity.daysCountToRepeat })}
                            {initialData.repeatingEntity.entityType === RepeatingEntityTypeEnum.WeekRepeatingEntity &&
                                t('taskForm.repeatsWeeklyOn', {
                                    days: initialData.repeatingEntity.repeatsOn
                                        .map((d) => daysOfWeek.find((dw) => dw.value === d)?.label ?? String(d))
                                        .join(', '),
                                })}
                            {initialData.repeatingEntity.entityType === RepeatingEntityTypeEnum.MonthRepeatingEntity &&
                                t('taskForm.repeatsMonthlyOnDay', { day: initialData.repeatingEntity.monthDayToRepeat })}
                            {initialData.repeatingEntity.entityType === RepeatingEntityTypeEnum.YearRepeatingEntity &&
                                t('taskForm.repeatsYearlyOnDay', { day: initialData.repeatingEntity.yearDayToRepeat })}
                        </Descriptions.Item>
                    </Descriptions>
                )}

                {isCreate && (
                    <>
                        <Form.Item name="addSchedule" valuePropName="checked" initialValue={false}>
                            <Checkbox>{t('taskForm.addSchedule')}</Checkbox>
                        </Form.Item>
                        {addSchedule && (
                            <Collapse defaultActiveKey={['schedule']}>
                                <Collapse.Panel header={t('taskForm.schedule')} key="schedule">
                                    <Form.Item
                                        name="scheduleType"
                                        label={t('taskForm.repeatType')}
                                        rules={[
                                            {
                                                required: true,
                                                message: t('taskForm.selectRepeatType'),
                                            },
                                        ]}
                                        initialValue={RepeatingEntityTypeEnum.DayRepeatingEntity}
                                    >
                                        <Select
                                            options={repeatTypes}
                                            placeholder={t('taskForm.selectType')}
                                        />
                                    </Form.Item>

                                    {scheduleType === RepeatingEntityTypeEnum.DayRepeatingEntity && (
                                        <Form.Item
                                            name="daysCountToRepeat"
                                            label={t('taskForm.repeatEveryNDays')}
                                            rules={[
                                                {
                                                    required: true,
                                                    message: t('taskForm.required'),
                                                },
                                            ]}
                                            initialValue={1}
                                        >
                                            <InputNumber min={1} style={{ width: '100%' }} />
                                        </Form.Item>
                                    )}

                                    {scheduleType === RepeatingEntityTypeEnum.WeekRepeatingEntity && (
                                        <Form.Item
                                            name="repeatsOn"
                                            label={t('taskForm.repeatOnDays')}
                                            rules={[
                                                {
                                                    required: true,
                                                    message: t('taskForm.selectAtLeastOneDay'),
                                                },
                                                {
                                                    type: 'array',
                                                    min: 1,
                                                    message: t('taskForm.selectAtLeastOneDay'),
                                                },
                                            ]}
                                        >
                                            <Checkbox.Group
                                                options={daysOfWeek}
                                                style={{ display: 'flex', flexWrap: 'wrap', gap: 8 }}
                                            />
                                        </Form.Item>
                                    )}

                                    {scheduleType === RepeatingEntityTypeEnum.MonthRepeatingEntity && (
                                        <Form.Item
                                            name="monthDayToRepeat"
                                            label={t('taskForm.dayOfMonth')}
                                            rules={[
                                                { required: true, message: t('taskForm.required') },
                                                {
                                                    type: 'number',
                                                    min: 1,
                                                    max: 31,
                                                    message: t('taskForm.between1And31'),
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

                                    {scheduleType === RepeatingEntityTypeEnum.YearRepeatingEntity && (
                                        <Form.Item
                                            name="yearDayToRepeat"
                                            label={t('taskForm.dayOfYear')}
                                            rules={[
                                                { required: true, message: t('taskForm.required') },
                                                {
                                                    type: 'number',
                                                    min: 1,
                                                    max: 366,
                                                    message: t('taskForm.between1And366'),
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

                                    <Form.Item name="endsOnMaxDate" label={t('taskForm.endsOnDate')}>
                                        <DatePicker format="YYYY-MM-DD" style={{ width: '100%' }} />
                                    </Form.Item>
                                    <Form.Item
                                        name="endsOnMaxOccurrences"
                                        label={t('taskForm.endsAfterOccurrences')}
                                    >
                                        <InputNumber
                                            min={1}
                                            placeholder={t('taskForm.noLimitPlaceholder')}
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
