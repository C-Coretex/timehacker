import type { FC } from 'react';
import { Form, Input, Slider, TimePicker, Row, Col } from 'antd';
import { useTranslation } from 'react-i18next';
import { getFixedPriorityMarks } from './constants';

const FixedTaskFields: FC = () => {
  const { t } = useTranslation();
  const priorityMarks = getFixedPriorityMarks(t);

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
        name="priority"
        label={t('taskForm.priority')}
        rules={[{ required: true, message: t('taskForm.priorityRequired') }]}
        initialValue={5}
      >
        <Slider min={1} max={10} marks={priorityMarks} />
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
  );
};

export default FixedTaskFields;
