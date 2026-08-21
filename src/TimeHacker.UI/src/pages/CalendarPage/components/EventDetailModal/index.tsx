import type { FC } from 'react';
import { Badge, Button, Descriptions, Divider, Modal, Space, Tag } from 'antd';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { ScheduleInfo } from '../ScheduleInfo';
import { argbToHex } from '../../../../utils/colorArgb';
import type { EventDetailModalProps } from './types';

export const EventDetailModal: FC<EventDetailModalProps> = ({
  open,
  onClose,
  event,
  scheduleData,
  loadingSchedule,
  timeDisplayFormat,
}) => {
  const { t } = useTranslation();
  const resource = event?.resource;
  const isCategory = resource?.type === 'category';
  const task = resource && resource.type !== 'category' ? resource.task : undefined;

  return (
    <Modal open={open} title={null} footer={null} onCancel={onClose} width={600}>
      {event && isCategory && (
        <div>
          <Space style={{ marginBottom: 16 }}>
            <Tag
              color={argbToHex(resource.category.color)}
              style={{ fontSize: 14, padding: '4px 12px' }}
            >
              {t('calendar.category')}
            </Tag>
          </Space>

          <Descriptions
            title={event.title}
            column={1}
            bordered
            size="small"
            styles={{ label: { fontWeight: 600, width: '30%' } }}
          >
            {event.description && (
              <Descriptions.Item label={t('calendar.descriptionLabel')}>
                {event.description}
              </Descriptions.Item>
            )}
            <Descriptions.Item label={t('calendar.startLabel')}>
              {dayjs(event.start).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
            <Descriptions.Item label={t('calendar.endLabel')}>
              {dayjs(event.end).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
          </Descriptions>

          <div style={{ marginTop: 16, textAlign: 'right' }}>
            <Button onClick={onClose}>{t('calendar.close')}</Button>
          </div>
        </div>
      )}

      {event && !isCategory && (
        <div>
          <Space style={{ marginBottom: 16 }}>
            <Tag
              color={resource?.type === 'fixed' ? 'green' : 'orange'}
              style={{ fontSize: 14, padding: '4px 12px' }}
            >
              {resource?.type === 'dynamic' ? t('calendar.dynamic') : t('calendar.fixed')}
            </Tag>
            <Badge
              count={task?.priority}
              showZero
              color={
                (task?.priority ?? 0) >= 8
                  ? '#ff4d4f'
                  : (task?.priority ?? 0) >= 5
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
            styles={{ label: { fontWeight: 600, width: '30%' } }}
          >
            {event.description && (
              <Descriptions.Item label={t('calendar.descriptionLabel')}>
                {event.description}
              </Descriptions.Item>
            )}
            <Descriptions.Item label={t('calendar.priorityLabel')}>
              {task?.priority ?? '-'}
            </Descriptions.Item>
            <Descriptions.Item label={t('calendar.startLabel')}>
              {dayjs(event.start).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
            <Descriptions.Item label={t('calendar.endLabel')}>
              {dayjs(event.end).format(`YYYY-MM-DD ${timeDisplayFormat}`)}
            </Descriptions.Item>
          </Descriptions>

          {resource?.type === 'fixed' && (
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

