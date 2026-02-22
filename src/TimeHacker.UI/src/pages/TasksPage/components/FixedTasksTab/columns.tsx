import { Button, Space, Tag, Typography } from 'antd';
import type { Breakpoint } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import type { TFunction } from 'i18next';
import type { ColumnType } from 'antd/es/table';
import type { FixedTaskDisplayModel, ScheduleEntityReturnModel } from '../../../../api/types';

const scheduleCell = (scheduleEntity: ScheduleEntityReturnModel | null, t: TFunction) => {
  if (!scheduleEntity) return <Tag>{t('tasks.oneTime')}</Tag>;

  const typeLabels: Record<number, string> = {
    1: t('tasks.daily'),
    2: t('tasks.weekly'),
    3: t('tasks.monthly'),
    4: t('tasks.yearly'),
  };

  return (
    <Space direction="vertical" size={0}>
      <Tag color="blue">{typeLabels[scheduleEntity.repeatingEntity.entityType]}</Tag>
      {scheduleEntity.endsOn && (
        <Typography.Text type="secondary" style={{ fontSize: 12 }}>
          {t('tasks.endsShort')}: {dayjs(scheduleEntity.endsOn).format('MMM D, YYYY')}
        </Typography.Text>
      )}
    </Space>
  );
};

export const getFixedTaskColumns = (
  isMobile: boolean,
  t: TFunction,
  onEdit: (task: FixedTaskDisplayModel) => void,
  onDelete: (id: string) => void
): ColumnType<FixedTaskDisplayModel>[] => [
  { title: t('tasks.name'), dataIndex: 'name', key: 'name' },
  { title: t('tasks.description'), dataIndex: 'description', key: 'description', responsive: ['md'] as Breakpoint[] },
  { title: t('tasks.priority'), dataIndex: 'priority', key: 'priority', width: isMobile ? 60 : undefined },
  {
    title: t('tasks.schedule'),
    dataIndex: 'scheduleEntity',
    key: 'schedule',
    responsive: ['lg'] as Breakpoint[],
    render: (scheduleEntity: ScheduleEntityReturnModel | null) => scheduleCell(scheduleEntity, t),
  },
  {
    title: isMobile ? t('tasks.start') : t('tasks.startTime'),
    dataIndex: 'startTimestamp',
    key: 'startTimestamp',
    render: (date: Dayjs) => date.format(isMobile ? 'MM/DD HH:mm' : 'YYYY-MM-DD HH:mm'),
  },
  {
    title: isMobile ? t('tasks.end') : t('tasks.endTime'),
    dataIndex: 'endTimestamp',
    key: 'endTimestamp',
    render: (date: Dayjs) => date.format(isMobile ? 'MM/DD HH:mm' : 'YYYY-MM-DD HH:mm'),
  },
  {
    title: t('tasks.actions'),
    key: 'actions',
    width: isMobile ? 80 : undefined,
    render: (_: unknown, task: FixedTaskDisplayModel) => (
      <>
        <Button type="link" icon={<EditOutlined />} onClick={() => onEdit(task)} size={isMobile ? 'small' : 'middle'}>
          {!isMobile && t('tasks.edit')}
        </Button>
        <Button type="link" danger icon={<DeleteOutlined />} onClick={() => onDelete(task.id)} size={isMobile ? 'small' : 'middle'}>
          {!isMobile && t('tasks.delete')}
        </Button>
      </>
    ),
  },
];
