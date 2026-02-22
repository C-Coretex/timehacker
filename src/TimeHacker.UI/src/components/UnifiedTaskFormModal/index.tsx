import { useEffect, useState } from 'react';
import type { FC } from 'react';
import { Modal, Form, Tabs, Calendar, Button, Row, Col } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { RepeatingEntityTypeEnum } from '../../api/types';
import type { FixedTaskFormData, InputDynamicTask, InputRepeatingEntityType, EndsOnModel } from '../../api/types';
import { minutesToTimeSpan, timeSpanToMinutes } from '../../utils/timeUtils';
import { useIsMobile } from '../../hooks/useIsMobile';

import FixedTaskFields from './FixedTaskFields';
import DynamicTaskFields from './DynamicTaskFields';
import ScheduleFormSection from './ScheduleFormSection';
import ScheduleReadOnlyInfo from './ScheduleReadOnlyInfo';
import type { TaskTab, ScheduleFormPayload, UnifiedTaskFormModalProps } from './types';

const TAB_ITEMS = (t: ReturnType<typeof useTranslation>['t']) => [
  { key: 'fixed', label: t('taskForm.fixedTask') },
  { key: 'dynamic', label: t('taskForm.dynamicTask') },
];

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

  const isEditFixed = !!initialFixedData;
  const isEditDynamic = !!initialDynamicData;
  const isEdit = isEditFixed || isEditDynamic;

  const [activeTab, setActiveTab] = useState<TaskTab>(initialTab);
  const [selectedDate, setSelectedDate] = useState<Dayjs>(
    defaultDate ? dayjs(defaultDate) : dayjs()
  );

  const addSchedule = Form.useWatch('addSchedule', form);
  const scheduleType = Form.useWatch('scheduleType', form);

  useEffect(() => {
    if (isEditFixed) setActiveTab('fixed');
    else if (isEditDynamic) setActiveTab('dynamic');
    else setActiveTab(initialTab);
  }, [isEditFixed, isEditDynamic, initialTab]);

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
        repeatingEntityType = { entityType: RepeatingEntityTypeEnum.DayRepeatingEntity, daysCountToRepeat: values.daysCountToRepeat ?? 1 };
        break;
      case RepeatingEntityTypeEnum.WeekRepeatingEntity:
        repeatingEntityType = { entityType: RepeatingEntityTypeEnum.WeekRepeatingEntity, repeatsOn: values.repeatsOn ?? [] };
        break;
      case RepeatingEntityTypeEnum.MonthRepeatingEntity:
        repeatingEntityType = { entityType: RepeatingEntityTypeEnum.MonthRepeatingEntity, monthDayToRepeat: values.monthDayToRepeat ?? 1 };
        break;
      case RepeatingEntityTypeEnum.YearRepeatingEntity:
        repeatingEntityType = { entityType: RepeatingEntityTypeEnum.YearRepeatingEntity, yearDayToRepeat: values.yearDayToRepeat ?? 1 };
        break;
      default:
        return undefined;
    }
    const endsOnModel: EndsOnModel | undefined =
      values.endsOnMaxDate || values.endsOnMaxOccurrences != null
        ? {
            maxDate: values.endsOnMaxDate ? dayjs(values.endsOnMaxDate).format('YYYY-MM-DD') : undefined,
            maxOccurrences: values.endsOnMaxOccurrences > 0 ? values.endsOnMaxOccurrences : undefined,
          }
        : undefined;
    return { repeatingEntityType, endsOnModel: endsOnModel ?? null };
  };

  const handleFinish = (values: Record<string, unknown>) => {
    if (activeTab === 'fixed') {
      const startTime = values.startTime as Dayjs;
      const endTime = values.endTime as Dayjs;
      const startTimestamp = selectedDate.hour(startTime.hour()).minute(startTime.minute()).second(0);
      const endTimestamp = selectedDate.hour(endTime.hour()).minute(endTime.minute()).second(0);
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

  const title = isEdit
    ? activeTab === 'fixed'
      ? t('taskForm.editTask')
      : t('dynamicTaskForm.editTask')
    : t('taskForm.addTask');

  const rightColumn = (
    <>
      <div style={{ marginBottom: 16 }}>
        <Calendar
          fullscreen={false}
          value={selectedDate}
          onSelect={(date) => setSelectedDate(date)}
        />
      </div>
      {!isEdit && activeTab === 'fixed' && <ScheduleFormSection />}
      {isEditFixed && initialFixedData?.scheduleEntity && (
        <ScheduleReadOnlyInfo scheduleEntity={initialFixedData.scheduleEntity} />
      )}
    </>
  );

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
            {isEdit ? t('taskForm.update') : t('taskForm.create')}
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
        items={TAB_ITEMS(t)}
        style={{ marginBottom: 16 }}
      />

      <Form form={form} onFinish={handleFinish} layout="vertical">
        {isMobile ? (
          <div>
            {activeTab === 'fixed' ? <FixedTaskFields /> : <DynamicTaskFields />}
            {rightColumn}
          </div>
        ) : (
          <Row gutter={24}>
            <Col span={14}>
              {activeTab === 'fixed' ? <FixedTaskFields /> : <DynamicTaskFields />}
            </Col>
            <Col span={10}>{rightColumn}</Col>
          </Row>
        )}
      </Form>
    </Modal>
  );
};

export default UnifiedTaskFormModal;
export type { ScheduleFormPayload } from './types';
