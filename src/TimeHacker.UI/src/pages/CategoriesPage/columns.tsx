import { Button, Tag } from 'antd';
import { EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { ColumnType } from 'antd/es/table';
import type { Breakpoint } from 'antd/es/_util/responsiveObserver';
import type { TFunction } from 'i18next';
import type { CategoryDisplayModel } from '../../api/types';
import { argbToHex } from '../../utils/colorArgb';

/** Desktop table only (phones get CategoryCards); actions stay pinned right when a narrow window scrolls it. */
export const getCategoryColumns = (
  t: TFunction,
  onEdit: (category: CategoryDisplayModel) => void,
  onDelete: (id: string) => void
): ColumnType<CategoryDisplayModel>[] => [
  {
    title: t('categories.color'),
    dataIndex: 'color',
    key: 'color',
    width: 72,
    render: (color: number) => (
      <span
        aria-hidden
        style={{
          display: 'inline-block',
          width: 18,
          height: 18,
          borderRadius: '50%',
          background: argbToHex(color),
          border: '1px solid var(--th-border)',
        }}
      />
    ),
  },
  { title: t('categories.name'), dataIndex: 'name', key: 'name' },
  {
    title: t('categories.description'),
    dataIndex: 'description',
    key: 'description',
    responsive: ['xl'] as Breakpoint[],
  },
  {
    title: t('categories.schedules'),
    key: 'schedules',
    render: (_: unknown, category: CategoryDisplayModel) =>
      category.schedules.length === 0 ? (
        <Tag>{t('categories.noSchedules')}</Tag>
      ) : (
        <Tag color="purple">{t('categories.scheduleCount', { count: category.schedules.length })}</Tag>
      ),
  },
  {
    title: t('categories.actions'),
    key: 'actions',
    fixed: 'right',
    render: (_: unknown, category: CategoryDisplayModel) => (
      <>
        <Button
          type="link"
          icon={<EditOutlined />}
          onClick={() => onEdit(category)}
        >
          {t('categories.edit')}
        </Button>
        <Button
          type="link"
          danger
          icon={<DeleteOutlined />}
          onClick={() => onDelete(category.id)}
        >
          {t('categories.delete')}
        </Button>
      </>
    ),
  },
];
