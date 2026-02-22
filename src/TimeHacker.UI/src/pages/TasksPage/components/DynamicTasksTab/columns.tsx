import { Button } from 'antd';
import type { Breakpoint } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { TFunction } from 'i18next';
import type { ColumnType } from 'antd/es/table';
import type { DynamicTaskReturnModel } from '../../../../api/types';

const formatDuration = (value: string): string => {
  if (!value) return '-';
  const parts = value.split(/[.:]/).map(Number);
  if (parts.length >= 3) {
    const [h, m] = parts;
    return `${h ?? 0}h ${m ?? 0}m`;
  }
  return value;
};

export const getDynamicTaskColumns = (
  isMobile: boolean,
  t: TFunction,
  onEdit: (task: DynamicTaskReturnModel) => void,
  onDelete: (id: string) => void
): ColumnType<DynamicTaskReturnModel>[] => [
  { title: t('tasks.name'), dataIndex: 'name', key: 'name' },
  { title: t('tasks.description'), dataIndex: 'description', key: 'description', responsive: ['md'] as Breakpoint[] },
  { title: t('tasks.priority'), dataIndex: 'priority', key: 'priority', width: isMobile ? 60 : undefined },
  {
    title: isMobile ? t('tasks.min') : t('tasks.minDuration'),
    dataIndex: 'minTimeToFinish',
    key: 'minTimeToFinish',
    render: formatDuration,
  },
  {
    title: isMobile ? t('tasks.max') : t('tasks.maxDuration'),
    dataIndex: 'maxTimeToFinish',
    key: 'maxTimeToFinish',
    render: formatDuration,
  },
  {
    title: isMobile ? t('tasks.optimal') : t('tasks.optimalDuration'),
    dataIndex: 'optimalTimeToFinish',
    key: 'optimalTimeToFinish',
    render: (v: string | null) => (v ? formatDuration(v) : '-'),
    responsive: ['md'] as Breakpoint[],
  },
  {
    title: t('tasks.actions'),
    key: 'actions',
    width: isMobile ? 80 : undefined,
    render: (_: unknown, task: DynamicTaskReturnModel) => (
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
