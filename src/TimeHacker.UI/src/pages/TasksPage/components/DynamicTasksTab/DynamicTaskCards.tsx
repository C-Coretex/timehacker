import type { FC } from 'react';
import { useTranslation } from 'react-i18next';
import type { DynamicTaskReturnModel } from '../../../../api/types';
import { ListCard, ListCardList, ListCardRange } from '../../../../components/ListCard';
import { PriorityTag } from '../../../../components/PriorityTag';
import { argbToHex } from '../../../../utils/colorArgb';
import { formatTimeSpan } from '../../../../utils/timeUtils';

interface DynamicTaskCardsProps {
  tasks: DynamicTaskReturnModel[];
  loading: boolean;
  onEdit: (task: DynamicTaskReturnModel) => void;
  onDelete: (id: string) => void;
  className?: string;
}

/** The dynamic tasks as list rows, for phones: priority and categories under the name, min – max time on the right. */
export const DynamicTaskCards: FC<DynamicTaskCardsProps> = ({ tasks, loading, onEdit, onDelete, className }) => {
  const { t } = useTranslation();

  return (
    <ListCardList
      className={className}
      items={tasks}
      loading={loading}
      emptyText={t('tasks.noDynamicTasks')}
      renderItem={(task) => (
        <ListCard
          tone="dynamic"
          title={task.name}
          categories={task.categories}
          description={task.description}
          accent={task.categories[0] ? argbToHex(task.categories[0].color) : undefined}
          meta={
            <>
              <PriorityTag priority={task.priority} />
              {task.categories.length > 0 && <strong>{task.categories.map((category) => category.name).join(', ')}</strong>}
            </>
          }
          aside={<ListCardRange from={formatTimeSpan(task.minTimeToFinish)} to={formatTimeSpan(task.maxTimeToFinish)} />}
          onOpen={() => onEdit(task)}
          onDelete={() => onDelete(task.id)}
          deleteLabel={t('tasks.delete')}
        />
      )}
    />
  );
};
