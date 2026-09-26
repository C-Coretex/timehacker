import type { FC } from 'react';
import { Form, Input } from 'antd';
import { useTranslation } from 'react-i18next';
import { CategorySelect } from '../CategorySelect';
import { PrioritySlider } from '../PrioritySlider';

/**
 * Name, priority and categories — the same for both task types. A dynamic task must be categorised (the
 * server rejects one without), a fixed task need not be.
 */
export const TaskBasicsFields: FC<{ categoryRequired: boolean }> = ({ categoryRequired }) => {
  const { t } = useTranslation();

  return (
    <>
      <Form.Item name="name" label={t('taskForm.taskName')} rules={[{ required: true, message: t('taskForm.taskNameRequired') }]}>
        <Input placeholder={t('taskForm.taskNamePlaceholder')} />
      </Form.Item>

      <Form.Item name="priority" label={t('taskForm.priority')} rules={[{ required: true, message: t('taskForm.priorityRequired') }]}>
        <PrioritySlider />
      </Form.Item>

      <CategorySelect required={categoryRequired} />
    </>
  );
};
