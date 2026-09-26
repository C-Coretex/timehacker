import { Button } from 'antd';
import type { Breakpoint } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { TFunction } from 'i18next';
import type { ColumnType } from 'antd/es/table';
import type { DynamicTaskReturnModel, CategoryReturnModel } from '../../../../api/types';
import { CategoryTags } from '../../../../components/CategoryTags';
import { PriorityTag } from '../../../../components/PriorityTag';
import { formatTimeSpan } from '../../../../utils/timeUtils';

/** Desktop table only (phones get DynamicTaskCards); actions stay pinned right when a narrow window scrolls it. */
export const getDynamicTaskColumns = (
  t: TFunction,
  onEdit: (task: DynamicTaskReturnModel) => void,
  onDelete: (id: string) => void
): ColumnType<DynamicTaskReturnModel>[] => [
  { title: t('tasks.name'), dataIndex: 'name', key: 'name' },
  { title: t('tasks.description'), dataIndex: 'description', key: 'description', responsive: ['xl'] as Breakpoint[] },
  {
    title: t('tasks.priority'),
    dataIndex: 'priority',
    key: 'priority',
    render: (priority: number) => <PriorityTag priority={priority} />,
  },
  {
    title: t('tasks.categories'),
    dataIndex: 'categories',
    key: 'categories',
    render: (categories: CategoryReturnModel[]) => <CategoryTags categories={categories} />,
  },
  {
    title: t('tasks.minDuration'),
    dataIndex: 'minTimeToFinish',
    key: 'minTimeToFinish',
    render: formatTimeSpan,
  },
  {
    title: t('tasks.maxDuration'),
    dataIndex: 'maxTimeToFinish',
    key: 'maxTimeToFinish',
    render: formatTimeSpan,
  },
  {
    title: t('tasks.optimalDuration'),
    dataIndex: 'optimalTimeToFinish',
    key: 'optimalTimeToFinish',
    render: formatTimeSpan,
    responsive: ['lg'] as Breakpoint[],
  },
  {
    title: t('tasks.actions'),
    key: 'actions',
    fixed: 'right',
    render: (_: unknown, task: DynamicTaskReturnModel) => (
      <>
        <Button type="link" icon={<EditOutlined />} onClick={() => onEdit(task)}>
          {t('tasks.edit')}
        </Button>
        <Button type="link" danger icon={<DeleteOutlined />} onClick={() => onDelete(task.id)}>
          {t('tasks.delete')}
        </Button>
      </>
    ),
  },
];
