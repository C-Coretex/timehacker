import type { FC } from 'react';
import { SyncOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import type { FixedTaskDisplayModel } from '../../../../api/types';
import { ListCard, ListCardList, ListCardRange } from '../../../../components/ListCard';
import { PriorityTag } from '../../../../components/PriorityTag';
import { useSettings } from '../../../../contexts/SettingsContext';
import { argbToHex } from '../../../../utils/colorArgb';
import { recurrenceTypeLabel } from '../../../../utils/describeRecurrence';

interface FixedTaskCardsProps {
  tasks: FixedTaskDisplayModel[];
  loading: boolean;
  onEdit: (task: FixedTaskDisplayModel) => void;
  onDelete: (id: string) => void;
  className?: string;
}

/** The fixed tasks as list rows, for phones: day, priority and recurrence under the name, the hours on the right. */
export const FixedTaskCards: FC<FixedTaskCardsProps> = ({ tasks, loading, onEdit, onDelete, className }) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();

  return (
    <ListCardList
      className={className}
      items={tasks}
      loading={loading}
      emptyText={t('tasks.noFixedTasks')}
      renderItem={(task) => (
        <ListCard
          tone="fixed"
          title={task.name}
          categories={task.categories}
          description={task.description}
          accent={task.categories[0] ? argbToHex(task.categories[0].color) : undefined}
          meta={
            <>
              <span>{task.startTimestamp.format('ddd, D MMM')}</span>
              <PriorityTag priority={task.priority} />
              {task.scheduleEntity && (
                <span>
                  <SyncOutlined /> {recurrenceTypeLabel(task.scheduleEntity.repeatingEntity.entityType, t)}
                </span>
              )}
            </>
          }
          aside={<ListCardRange from={task.startTimestamp.format(timeDisplayFormat)} to={task.endTimestamp.format(timeDisplayFormat)} />}
          onOpen={() => onEdit(task)}
          onDelete={() => onDelete(task.id)}
          deleteLabel={t('tasks.delete')}
        />
      )}
    />
  );
};
