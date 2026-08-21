import { Button, Space, Tag, Typography } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import type { ColumnType } from 'antd/es/table';
import type { Breakpoint } from 'antd/es/_util/responsiveObserver';
import type { TFunction } from 'i18next';
import type { CategoryDisplayModel, ScheduleEntityReturnModel } from '../../api/types';
import { argbToHex } from '../../utils/colorArgb';
import { recurrenceTypeLabel } from '../../utils/describeRecurrence';

const scheduleCell = (scheduleEntity: ScheduleEntityReturnModel | null, t: TFunction) => {
  // Without a schedule a category has no day to land on, so it never reaches the calendar.
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

export const getCategoryColumns = (
  isMobile: boolean,
  t: TFunction,
  onEdit: (category: CategoryDisplayModel) => void,
  onDelete: (id: string) => void
): ColumnType<CategoryDisplayModel>[] => [
  {
    title: t('categories.color'),
    dataIndex: 'color',
    key: 'color',
    width: isMobile ? 48 : 72,
    render: (color: number) => (
      <span
        aria-hidden
        style={{
          display: 'inline-block',
          width: 18,
          height: 18,
          borderRadius: 4,
          background: argbToHex(color),
          border: '1px solid rgba(0,0,0,0.15)',
        }}
      />
    ),
  },
  { title: t('categories.name'), dataIndex: 'name', key: 'name' },
  {
    title: t('categories.description'),
    dataIndex: 'description',
    key: 'description',
    responsive: ['md'] as Breakpoint[],
  },
  {
    title: t('categories.timeWindow'),
    key: 'timeWindow',
    render: (_: unknown, category: CategoryDisplayModel) =>
      `${(category.startTime as Dayjs).format('HH:mm')} – ${(category.endTime as Dayjs).format('HH:mm')}`,
  },
  {
    title: t('categories.schedule'),
    dataIndex: 'scheduleEntity',
    key: 'schedule',
    responsive: ['lg'] as Breakpoint[],
    render: (scheduleEntity: ScheduleEntityReturnModel | null) => scheduleCell(scheduleEntity, t),
  },
  {
    title: t('categories.actions'),
    key: 'actions',
    width: isMobile ? 80 : undefined,
    render: (_: unknown, category: CategoryDisplayModel) => (
      <>
        <Button
          type="link"
          icon={<EditOutlined />}
          onClick={() => onEdit(category)}
          size={isMobile ? 'small' : 'middle'}
        >
          {!isMobile && t('categories.edit')}
        </Button>
        <Button
          type="link"
          danger
          icon={<DeleteOutlined />}
          onClick={() => onDelete(category.id)}
          size={isMobile ? 'small' : 'middle'}
        >
          {!isMobile && t('categories.delete')}
        </Button>
      </>
    ),
  },
];
