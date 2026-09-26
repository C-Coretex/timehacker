import type { FC } from 'react';
import { Button, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import type { TFunction } from 'i18next';
import type { CategoryDisplayModel, CategoryScheduleDisplayModel } from '../../api/types';
import { getCategoryScheduleColumns } from './scheduleColumns';

interface SchedulesPanelProps {
  category: CategoryDisplayModel;
  t: TFunction;
  onAdd: (category: CategoryDisplayModel) => void;
  onEdit: (category: CategoryDisplayModel, schedule: CategoryScheduleDisplayModel) => void;
  onDelete: (schedule: CategoryScheduleDisplayModel) => void;
}

/** The expanded row of the categories table: every time window this category occupies. */
export const SchedulesPanel: FC<SchedulesPanelProps> = ({
  category,
  t,
  onAdd,
  onEdit,
  onDelete,
}) => (
  <div style={{ padding: '0 16px 12px' }}>
    <Table
      columns={getCategoryScheduleColumns(
        t,
        (schedule) => onEdit(category, schedule),
        onDelete
      )}
      dataSource={category.schedules}
      rowKey="id"
      pagination={false}
      size="small"
      locale={{
        emptyText: (
          <Typography.Text type="secondary">{t('categorySchedules.noSchedules')}</Typography.Text>
        ),
      }}
    />
    <Button
      type="dashed"
      icon={<PlusOutlined />}
      onClick={() => onAdd(category)}
      size="small"
      style={{ marginTop: 8 }}
    >
      {t('categorySchedules.addSchedule')}
    </Button>
  </div>
);
