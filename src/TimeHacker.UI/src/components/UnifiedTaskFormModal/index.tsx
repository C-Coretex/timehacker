import { useEffect, useState } from 'react';
import type { FC } from 'react';
import { Form, Input } from 'antd';
import { useTranslation } from 'react-i18next';
import { buildSchedulePayload } from '../../utils/buildSchedulePayload';
import { ResponsiveFormShell } from '../ResponsiveFormShell';
import { WhenFields } from '../WhenFields';
import { DurationFields } from './DurationFields';
import { TaskBasicsFields } from './TaskBasicsFields';
import { TaskTypeToggle } from './TaskTypeToggle';
import { dynamicTaskValues, fixedTaskValues, prefillValues, toDynamicTaskInput, toFixedTaskFormData } from './taskFormValues';
import type { TaskFormValues, TaskTab, UnifiedTaskFormModalProps } from './types';
import './styles.css';

/**
 * Create or edit a task of either type. A fixed task is placed by the calendar card (day, time range and,
 * on create, a recurrence); a dynamic one by its durations. Name, priority, categories and description are
 * shared, so they survive switching the type.
 */
export const UnifiedTaskFormModal: FC<UnifiedTaskFormModalProps> = ({
  open,
  onCancel,
  onSaveFixed,
  onSaveDynamic,
  initialFixedData,
  initialDynamicData,
  initialTab = 'fixed',
  prefill,
}) => {
  const [form] = Form.useForm<TaskFormValues>();
  const { t } = useTranslation();
  const [type, setType] = useState<TaskTab>(initialTab);
  const isEdit = !!initialFixedData || !!initialDynamicData;

  useEffect(() => {
    if (!open) return;
    form.resetFields();
    if (initialFixedData) {
      setType('fixed');
      form.setFieldsValue(fixedTaskValues(initialFixedData));
    } else if (initialDynamicData) {
      setType('dynamic');
      form.setFieldsValue(dynamicTaskValues(initialDynamicData));
    } else {
      setType(initialTab);
      form.setFieldsValue(prefillValues(prefill));
    }
  }, [open, form, initialFixedData, initialDynamicData, initialTab, prefill]);

  const handleFinish = (values: TaskFormValues) => {
    if (type === 'fixed') {
      onSaveFixed(toFixedTaskFormData(values), initialFixedData?.id, isEdit ? undefined : buildSchedulePayload(values));
    } else {
      onSaveDynamic(toDynamicTaskInput(values), initialDynamicData?.id);
    }
  };

  return (
    <ResponsiveFormShell
      open={open}
      onCancel={onCancel}
      title={isEdit ? t('taskForm.editTask') : t('taskForm.addTask')}
      submitLabel={isEdit ? t('taskForm.update') : t('taskForm.addTask')}
      isEdit={isEdit}
      onSubmit={() => form.submit()}
    >
      <Form form={form} layout="vertical" requiredMark={false} onFinish={handleFinish} className="th-task-form">
        <TaskTypeToggle value={type} onChange={setType} disabled={isEdit} />

        <div className="th-task-form__columns">
          <div className="th-task-form__main">
            <TaskBasicsFields categoryRequired={type === 'dynamic'} />
          </div>
          <div className="th-task-form__side">
            {type === 'fixed' ? (
              <WhenFields isEdit={isEdit} scheduleEntity={initialFixedData?.scheduleEntity} />
            ) : (
              <DurationFields />
            )}
          </div>
        </div>

        <Form.Item name="description" label={t('taskForm.description')}>
          <Input.TextArea rows={4} placeholder={t('taskForm.descriptionPlaceholder')} />
        </Form.Item>
      </Form>
    </ResponsiveFormShell>
  );
};

export type { ScheduleFormPayload, TaskPrefill } from './types';
