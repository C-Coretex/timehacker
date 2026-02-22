import type { FC } from 'react';
import { Badge, Button, Descriptions, Divider, Modal, Space, Tag } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import ScheduleInfo from '../ScheduleInfo';
import type { EventDetailModalProps } from './types';

const EventDetailModal: FC<EventDetailModalProps> = ({
  open,
  onClose,
  event,
  scheduleData,
  loadingSchedule,
  timeDisplayFormat,
}) => {
  const { t } = useTranslation();

  return (
    <Modal open={open} title={null} footer={null} onCancel={onClose} width={600}>
      {event && (
        <div>
          <Space style={{ marginBottom: 16 }}>
            <Tag
              color={event.resource?.type === 'fixed' ? 'green' : 'orange'}
              style={{ fontSize: 14, padding: '4px 12px' }}
            >
              {event.resource?.type === 'dynamic' ? t('calendar.dynamic') : t('calendar.fixed')}
            </Tag>
            <Badge
              count={event.resource?.task.priority}
              showZero
              color={
                (event.resource?.task.priority ?? 0) >= 8
                  ? '#ff4d4f'
                  : (event.resource?.task.priority ?? 0) >= 5
                    ? '#faad14'
                    : '#52c41a'
              }
            />
          </Space>

          <Descriptions
            title={event.title}
            column={1}
            bordered
            size="small"
            labelStyle={{ fontWeight: 600, width: '30%' }}
          >
            {event.description && (
              <Descriptions.Item label={t('calendar.descriptionLabel')}>
                {event.description}
              </Descriptions.Item>
            )}
            <Descriptions.Item label={t('calendar.priorityLabel')}>
              {event.resource?.task.priority ?? '-'}
            </Descriptions.Item>
            <Descriptions.Item label={t('calendar.startLabel')}>
              {dayjs(event.start).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
            <Descriptions.Item label={t('calendar.endLabel')}>
              {dayjs(event.end).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
          </Descriptions>

          {event.resource?.isFixed && (
            <>
              <Divider />
              <ScheduleInfo scheduleData={scheduleData} loading={loadingSchedule} />
            </>
          )}

          <div style={{ marginTop: 16, textAlign: 'right' }}>
            <Button onClick={onClose}>{t('calendar.close')}</Button>
          </div>
        </div>
      )}
    </Modal>
  );
};

export default EventDetailModal;
