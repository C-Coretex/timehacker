import type { FC } from 'react';
import { Form, Input, Slider, InputNumber, Row, Col } from 'antd';
import { useTranslation } from 'react-i18next';
import { getDynamicPriorityMarks } from './constants';

const DynamicTaskFields: FC = () => {
  const { t } = useTranslation();
  const priorityMarks = getDynamicPriorityMarks(t);

  return (
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

      <Form.Item
        name="dynamicPriority"
        label={t('dynamicTaskForm.priority')}
        rules={[{ required: true, message: t('dynamicTaskForm.priorityRequired') }]}
        initialValue={128}
      >
        <Slider min={0} max={255} marks={priorityMarks} />
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
  );
};

export default DynamicTaskFields;
