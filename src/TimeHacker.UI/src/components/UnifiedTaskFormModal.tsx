import { useEffect, useState } from 'react';
import type { FC } from 'react';
import {
  Modal,
  Form,
  Input,
  InputNumber,
  Slider,
  Tabs,
  Calendar,
  Checkbox,
  Select,
  TimePicker,
  DatePicker,
  Row,
  Col,
  Button,
  Descriptions,
  theme,
} from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { RepeatingEntityTypeEnum } from '../api/types';
import type {
  FixedTaskFormData,
  InputDynamicTask,
  InputRepeatingEntityType,
  EndsOnModel,
  ReturnRepeatingEntityModel,
  DynamicTaskReturnModel,
} from '../api/types';
import { useIsMobile } from '../hooks/useIsMobile';

export type ScheduleFormPayload = {
  repeatingEntityType: InputRepeatingEntityType;
  endsOnModel?: EndsOnModel | null;
};

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

type TaskTab = 'fixed' | 'dynamic';

interface UnifiedTaskFormModalProps {
  open: boolean;
  onCancel: () => void;
  onSaveFixed: (data: FixedTaskFormData, id?: string, schedule?: ScheduleFormPayload) => void;
  onSaveDynamic: (data: InputDynamicTask, id?: string) => void;
  initialFixedData?: FixedTaskFormData & { id: string; repeatingEntity?: ReturnRepeatingEntityModel | null };
  initialDynamicData?: DynamicTaskReturnModel | null;
  initialTab?: TaskTab;
  defaultDate?: Date;
}

const UnifiedTaskFormModal: FC<UnifiedTaskFormModalProps> = ({
  open,
  onCancel,
  onSaveFixed,
  onSaveDynamic,
  initialFixedData,
  initialDynamicData,
  initialTab = 'fixed',
  defaultDate,
}) => {
  const [form] = Form.useForm();
  const { t } = useTranslation();
  const { isMobile } = useIsMobile();
  const { token } = theme.useToken();

  const isEditFixed = !!initialFixedData;
  const isEditDynamic = !!initialDynamicData;
  const isEdit = isEditFixed || isEditDynamic;

  const [activeTab, setActiveTab] = useState<TaskTab>(initialTab);
  const [selectedDate, setSelectedDate] = useState<Dayjs>(
    defaultDate ? dayjs(defaultDate) : dayjs()
  );

  const addSchedule = Form.useWatch('addSchedule', form);
  const scheduleType = Form.useWatch('scheduleType', form);

  const repeatTypes = [
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

  // Determine initial tab based on which data is provided
  useEffect(() => {
    if (isEditFixed) {
      setActiveTab('fixed');
    } else if (isEditDynamic) {
      setActiveTab('dynamic');
    } else {
      setActiveTab(initialTab);
    }
  }, [isEditFixed, isEditDynamic, initialTab]);

  // Populate form when editing
  useEffect(() => {
    if (!open) return;

    if (initialFixedData) {
      const start = initialFixedData.startTimestamp ? dayjs(initialFixedData.startTimestamp) : null;
      const end = initialFixedData.endTimestamp ? dayjs(initialFixedData.endTimestamp) : null;
      if (start) setSelectedDate(start);
      form.setFieldsValue({
        name: initialFixedData.name,
        description: initialFixedData.description,
        priority: initialFixedData.priority,
        startTime: start,
        endTime: end,
      });
    } else if (initialDynamicData) {
      form.setFieldsValue({
        name: initialDynamicData.name,
        description: initialDynamicData.description ?? '',
        dynamicPriority: initialDynamicData.priority,
        minMinutes: timeSpanToMinutes(initialDynamicData.minTimeToFinish),
        maxMinutes: timeSpanToMinutes(initialDynamicData.maxTimeToFinish),
        optimalMinutes: initialDynamicData.optimalTimeToFinish
          ? timeSpanToMinutes(initialDynamicData.optimalTimeToFinish)
          : null,
      });
    } else {
      form.resetFields();
      setSelectedDate(defaultDate ? dayjs(defaultDate) : dayjs());
    }
  }, [initialFixedData, initialDynamicData, open, form, defaultDate]);

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
    if (activeTab === 'fixed') {
      const startTime = values.startTime as Dayjs;
      const endTime = values.endTime as Dayjs;

      // Combine selected date with time
      const startTimestamp = selectedDate
        .hour(startTime.hour())
        .minute(startTime.minute())
        .second(0);
      const endTimestamp = selectedDate
        .hour(endTime.hour())
        .minute(endTime.minute())
        .second(0);

      const taskData: FixedTaskFormData = {
        name: values.name as string,
        description: values.description as string,
        priority: values.priority as number,
        startTimestamp,
        endTimestamp,
      };
      const schedule = !isEdit ? buildSchedulePayload() : undefined;
      onSaveFixed(taskData, initialFixedData?.id, schedule);
    } else {
      const payload: InputDynamicTask = {
        name: values.name as string,
        description: (values.description as string) || undefined,
        priority: values.dynamicPriority as number,
        minTimeToFinish: minutesToTimeSpan(values.minMinutes as number),
        maxTimeToFinish: minutesToTimeSpan(values.maxMinutes as number),
        optimalTimeToFinish:
          values.optimalMinutes != null && (values.optimalMinutes as number) > 0
            ? minutesToTimeSpan(values.optimalMinutes as number)
            : undefined,
      };
      onSaveDynamic(payload, initialDynamicData?.id);
    }
  };

  const fixedPriorityMarks = {
    1: t('taskForm.priorityLow'),
    5: t('taskForm.priorityNormal'),
    10: t('taskForm.priorityHigh'),
  };

  const dynamicPriorityMarks = {
    0: t('taskForm.priorityLow'),
    128: t('taskForm.priorityNormal'),
    255: t('taskForm.priorityHigh'),
  };

  const title = isEdit
    ? activeTab === 'fixed'
      ? t('taskForm.editTask')
      : t('dynamicTaskForm.editTask')
    : t('taskForm.addTask');

  const submitLabel = isEdit ? t('taskForm.update') : t('taskForm.create');

  const leftColumn = (
    <>
      <Form.Item
        name="name"
        label={t('taskForm.taskName')}
        rules={[{ required: true, message: t('taskForm.taskNameRequired') }]}
      >
        <Input placeholder={t('taskForm.taskName')} />
      </Form.Item>

      <Form.Item name="description" label={t('taskForm.description')}>
        <Input.TextArea rows={3} placeholder={t('taskForm.description')} />
      </Form.Item>

      {activeTab === 'fixed' ? (
        <>
          <Form.Item
            name="priority"
            label={t('taskForm.priority')}
            rules={[{ required: true, message: t('taskForm.priorityRequired') }]}
            initialValue={5}
          >
            <Slider min={1} max={10} marks={fixedPriorityMarks} />
          </Form.Item>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                name="startTime"
                label={t('taskForm.startTimeLabel')}
                rules={[{ required: true, message: t('taskForm.startTimeRequired') }]}
              >
                <TimePicker format="HH:mm" style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item
                name="endTime"
                label={t('taskForm.endTimeLabel')}
                rules={[{ required: true, message: t('taskForm.endTimeRequired') }]}
              >
                <TimePicker format="HH:mm" style={{ width: '100%' }} />
              </Form.Item>
            </Col>
          </Row>
        </>
      ) : (
        <>
          <Form.Item
            name="dynamicPriority"
            label={t('dynamicTaskForm.priority')}
            rules={[{ required: true, message: t('dynamicTaskForm.priorityRequired') }]}
            initialValue={128}
          >
            <Slider min={0} max={255} marks={dynamicPriorityMarks} />
          </Form.Item>

          <Row gutter={16}>
            <Col span={8}>
              <Form.Item
                name="minMinutes"
                label={t('dynamicTaskForm.minDuration')}
                rules={[{ required: true, message: t('dynamicTaskForm.minDurationRequired') }]}
              >
                <InputNumber min={1} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item
                name="maxMinutes"
                label={t('dynamicTaskForm.maxDuration')}
                rules={[{ required: true, message: t('dynamicTaskForm.maxDurationRequired') }]}
              >
                <InputNumber min={1} style={{ width: '100%' }} />
              </Form.Item>
            </Col>
            <Col span={8}>
              <Form.Item name="optimalMinutes" label={t('dynamicTaskForm.optimalDuration')}>
                <InputNumber min={0} style={{ width: '100%' }} placeholder={t('dynamicTaskForm.optional')} />
              </Form.Item>
            </Col>
          </Row>
        </>
      )}
    </>
  );

  const rightColumn = (
    <>
      <div style={{ marginBottom: 16 }}>
        <Calendar
          fullscreen={false}
          value={selectedDate}
          onSelect={(date) => setSelectedDate(date)}
        />
      </div>

      {/* Recurring schedule - create mode, fixed tab only */}
      {!isEdit && activeTab === 'fixed' && (
        <div style={{ marginTop: 8 }}>
          <Form.Item name="addSchedule" valuePropName="checked" initialValue={false} style={{ marginBottom: 8 }}>
            <Checkbox>{t('taskForm.repeat')}</Checkbox>
          </Form.Item>

          {addSchedule && (
            <div
              style={{
                background: token.colorBgLayout,
                borderRadius: token.borderRadius,
                border: `1px solid ${token.colorBorderSecondary}`,
                padding: '12px 12px 4px',
              }}
            >
              <Form.Item
                name="scheduleType"
                label={t('taskForm.repeatType')}
                rules={[{ required: true, message: t('taskForm.selectRepeatType') }]}
                initialValue={RepeatingEntityTypeEnum.DayRepeatingEntity}
                style={{ marginBottom: 8 }}
              >
                <Select options={repeatTypes} placeholder={t('taskForm.selectType')} size="small" />
              </Form.Item>

              {scheduleType === RepeatingEntityTypeEnum.DayRepeatingEntity && (
                <Form.Item
                  name="daysCountToRepeat"
                  label={t('taskForm.repeatEveryNDays')}
                  rules={[{ required: true, message: t('taskForm.required') }]}
                  initialValue={1}
                  style={{ marginBottom: 8 }}
                >
                  <InputNumber min={1} style={{ width: '100%' }} size="small" />
                </Form.Item>
              )}

              {scheduleType === RepeatingEntityTypeEnum.WeekRepeatingEntity && (
                <Form.Item
                  name="repeatsOn"
                  label={t('taskForm.repeatOnDays')}
                  rules={[
                    { required: true, message: t('taskForm.selectAtLeastOneDay') },
                    { type: 'array', min: 1, message: t('taskForm.selectAtLeastOneDay') },
                  ]}
                  style={{ marginBottom: 8 }}
                >
                  <Checkbox.Group
                    options={daysOfWeek}
                    style={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}
                  />
                </Form.Item>
              )}

              {scheduleType === RepeatingEntityTypeEnum.MonthRepeatingEntity && (
                <Form.Item
                  name="monthDayToRepeat"
                  label={t('taskForm.dayOfMonth')}
                  rules={[
                    { required: true, message: t('taskForm.required') },
                    { type: 'number', min: 1, max: 31, message: t('taskForm.between1And31') },
                  ]}
                  initialValue={1}
                  style={{ marginBottom: 8 }}
                >
                  <InputNumber min={1} max={31} style={{ width: '100%' }} size="small" />
                </Form.Item>
              )}

              {scheduleType === RepeatingEntityTypeEnum.YearRepeatingEntity && (
                <Form.Item
                  name="yearDayToRepeat"
                  label={t('taskForm.dayOfYear')}
                  rules={[
                    { required: true, message: t('taskForm.required') },
                    { type: 'number', min: 1, max: 366, message: t('taskForm.between1And366') },
                  ]}
                  initialValue={1}
                  style={{ marginBottom: 8 }}
                >
                  <InputNumber min={1} max={366} style={{ width: '100%' }} size="small" />
                </Form.Item>
              )}

              <Form.Item name="endsOnMaxDate" label={t('taskForm.endsOnDate')} style={{ marginBottom: 8 }}>
                <DatePicker format="YYYY-MM-DD" style={{ width: '100%' }} size="small" />
              </Form.Item>
              <Form.Item name="endsOnMaxOccurrences" label={t('taskForm.endsAfterOccurrences')} style={{ marginBottom: 8 }}>
                <InputNumber
                  min={1}
                  placeholder={t('taskForm.noLimitPlaceholder')}
                  style={{ width: '100%' }}
                  size="small"
                />
              </Form.Item>
            </div>
          )}
        </div>
      )}

      {/* Show existing schedule in edit mode */}
      {isEditFixed && initialFixedData?.repeatingEntity && (
        <Descriptions
          title={t('taskForm.currentSchedule')}
          bordered
          size="small"
          column={1}
          style={{ marginTop: 12 }}
        >
          <Descriptions.Item label={t('taskForm.repeatType')}>
            {initialFixedData.repeatingEntity.entityType === RepeatingEntityTypeEnum.DayRepeatingEntity &&
              t('taskForm.repeatsEveryNDays', { count: initialFixedData.repeatingEntity.daysCountToRepeat })}
            {initialFixedData.repeatingEntity.entityType === RepeatingEntityTypeEnum.WeekRepeatingEntity &&
              t('taskForm.repeatsWeeklyOn', {
                days: initialFixedData.repeatingEntity.repeatsOn
                  .map((d) => daysOfWeek.find((dw) => dw.value === d)?.label ?? String(d))
                  .join(', '),
              })}
            {initialFixedData.repeatingEntity.entityType === RepeatingEntityTypeEnum.MonthRepeatingEntity &&
              t('taskForm.repeatsMonthlyOnDay', { day: initialFixedData.repeatingEntity.monthDayToRepeat })}
            {initialFixedData.repeatingEntity.entityType === RepeatingEntityTypeEnum.YearRepeatingEntity &&
              t('taskForm.repeatsYearlyOnDay', { day: initialFixedData.repeatingEntity.yearDayToRepeat })}
          </Descriptions.Item>
        </Descriptions>
      )}
    </>
  );

  const tabItems = [
    { key: 'fixed', label: t('taskForm.fixedTask') },
    { key: 'dynamic', label: t('taskForm.dynamicTask') },
  ];

  return (
    <Modal
      open={open}
      forceRender
      destroyOnHidden
      title={title}
      width={isMobile ? '100%' : 800}
      onCancel={onCancel}
      footer={
        <div style={{ display: 'flex', flexDirection: 'column', gap: 4 }}>
          <Button type="primary" block size="large" onClick={() => form.submit()}>
            {submitLabel}
          </Button>
          <Button type="text" block size="small" onClick={onCancel}>
            {t('taskForm.cancel')}
          </Button>
        </div>
      }
    >
      <Tabs
        activeKey={activeTab}
        onChange={(key) => {
          if (!isEdit) {
            setActiveTab(key as TaskTab);
            form.resetFields();
          }
        }}
        items={tabItems}
        style={{ marginBottom: 16 }}
      />

      <Form form={form} onFinish={handleFinish} layout="vertical">
        {isMobile ? (
          <div>
            {leftColumn}
            {rightColumn}
          </div>
        ) : (
          <Row gutter={24}>
            <Col span={14}>{leftColumn}</Col>
            <Col span={10}>{rightColumn}</Col>
          </Row>
        )}
      </Form>
    </Modal>
  );
};

export default UnifiedTaskFormModal;
