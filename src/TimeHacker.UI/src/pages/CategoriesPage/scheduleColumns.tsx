import { Button, Space, Tag, Typography } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import type { ColumnType } from 'antd/es/table';
import type { Breakpoint } from 'antd/es/_util/responsiveObserver';
import type { TFunction } from 'i18next';
import type { CategoryScheduleDisplayModel, ScheduleEntityReturnModel } from '../../api/types';
import { recurrenceTypeLabel } from '../../utils/describeRecurrence';

const recurrenceCell = (scheduleEntity: ScheduleEntityReturnModel | null, t: TFunction) => {
  // Without a recurrence the window still lands on its own date — it just never repeats.
  if (!scheduleEntity) return <Tag>{t('categories.notScheduled')}</Tag>;

  return (
    <Space orientation="vertical" size={0}>
      <Tag color="blue">{recurrenceTypeLabel(scheduleEntity.repeatingEntity.entityType, t)}</Tag>
      {scheduleEntity.endsOn && (
        <Typography.Text type="secondary" style={{ fontSize: 12 }}>
          {t('tasks.endsShort')}: {dayjs(scheduleEntity.endsOn).format('MMM D, YYYY')}
        </Typography.Text>
      )}
    </Space>
  );
};

export const getCategoryScheduleColumns = (
  isMobile: boolean,
  t: TFunction,
  onEdit: (schedule: CategoryScheduleDisplayModel) => void,
  onDelete: (schedule: CategoryScheduleDisplayModel) => void
): ColumnType<CategoryScheduleDisplayModel>[] => [
  {
    title: t('categorySchedules.description'),
    dataIndex: 'description',
    key: 'description',
    // Optional — the category names the window, so most rows leave this blank.
    render: (description: string | null) =>
      description || (
        <Typography.Text type="secondary">{t('categorySchedules.noDescription')}</Typography.Text>
      ),
  },
  {
    title: t('categorySchedules.date'),
    dataIndex: 'date',
    key: 'date',
    render: (date: Dayjs) => date.format('MMM D, YYYY'),
  },
  {
    title: t('categorySchedules.timeWindow'),
    key: 'timeWindow',
    render: (_: unknown, schedule: CategoryScheduleDisplayModel) =>
      `${schedule.startTime.format('HH:mm')} – ${schedule.endTime.format('HH:mm')}`,
  },
  {
    title: t('categorySchedules.recurrence'),
    dataIndex: 'scheduleEntity',
    key: 'recurrence',
    responsive: ['md'] as Breakpoint[],
    render: (scheduleEntity: ScheduleEntityReturnModel | null) => recurrenceCell(scheduleEntity, t),
  },
  {
    title: t('categorySchedules.actions'),
    key: 'actions',
    width: isMobile ? 80 : undefined,
    render: (_: unknown, schedule: CategoryScheduleDisplayModel) => (
      <>
        <Button
          type="link"
          icon={<EditOutlined />}
          onClick={() => onEdit(schedule)}
          size={isMobile ? 'small' : 'middle'}
        >
          {!isMobile && t('categorySchedules.edit')}
        </Button>
        <Button
          type="link"
          danger
          icon={<DeleteOutlined />}
          onClick={() => onDelete(schedule)}
          size={isMobile ? 'small' : 'middle'}
        >
          {!isMobile && t('categorySchedules.delete')}
        </Button>
      </>
    ),
  },
];
