import { useState } from 'react';
import type { FC } from 'react';
import { Button } from 'antd';
import { DownOutlined, PlusOutlined, RightOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import type { CategoryDisplayModel, CategoryScheduleDisplayModel } from '../../api/types';
import { ListCard, ListCardList } from '../../components/ListCard';
import { useSettings } from '../../contexts/SettingsContext';
import { argbToHex } from '../../utils/colorArgb';
import { recurrenceTypeLabel } from '../../utils/describeRecurrence';
import './styles.css';

interface WindowActions {
  onAddWindow: (category: CategoryDisplayModel) => void;
  onEditWindow: (category: CategoryDisplayModel, schedule: CategoryScheduleDisplayModel) => void;
  onDeleteWindow: (schedule: CategoryScheduleDisplayModel) => void;
}

interface CategoryCardsProps extends WindowActions {
  categories: CategoryDisplayModel[];
  loading: boolean;
  onEdit: (category: CategoryDisplayModel) => void;
  onDelete: (id: string) => void;
  className?: string;
}

/** A category's time windows under its card: collapsed to a count until opened, then one row each and "Add". */
const CategoryWindows: FC<WindowActions & { category: CategoryDisplayModel }> = ({
  category,
  onAddWindow,
  onEditWindow,
  onDeleteWindow,
}) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();
  const [expanded, setExpanded] = useState(false);
  const { schedules } = category;

  return (
    <div className="th-category-windows">
      {schedules.length > 0 && (
        <button type="button" className="th-category-windows__toggle" aria-expanded={expanded} onClick={() => setExpanded((open) => !open)}>
          {expanded ? <DownOutlined /> : <RightOutlined />}
          {t('categories.scheduleCount', { count: schedules.length })}
        </button>
      )}
      {expanded &&
        schedules.map((schedule) => (
          <ListCard
            key={schedule.id}
            tone="plain"
            accent={argbToHex(category.color)}
            title={`${schedule.date.format('D MMM YYYY')} · ${schedule.startTime.format(timeDisplayFormat)} – ${schedule.endTime.format(timeDisplayFormat)}`}
            description={schedule.description}
            meta={
              schedule.scheduleEntity
                ? recurrenceTypeLabel(schedule.scheduleEntity.repeatingEntity.entityType, t)
                : t('categories.notScheduled')
            }
            onOpen={() => onEditWindow(category, schedule)}
            onDelete={() => onDeleteWindow(schedule)}
            deleteLabel={t('categorySchedules.delete')}
          />
        ))}
      {(expanded || schedules.length === 0) && (
        <Button type="dashed" size="small" icon={<PlusOutlined />} className="th-category-windows__add" onClick={() => onAddWindow(category)}>
          {t('categorySchedules.addSchedule')}
        </Button>
      )}
    </div>
  );
};

/** The categories as cards, for phones: tap to edit, 🗑 to delete, and each card opens onto its time windows. */
export const CategoryCards: FC<CategoryCardsProps> = ({ categories, loading, onEdit, onDelete, className, ...windowActions }) => {
  const { t } = useTranslation();

  return (
    <ListCardList
      className={className}
      items={categories}
      loading={loading}
      emptyText={t('categories.noCategories')}
      renderItem={(category) => (
        <ListCard
          tone="plain"
          accent={argbToHex(category.color)}
          title={category.name}
          description={category.description}
          onOpen={() => onEdit(category)}
          onDelete={() => onDelete(category.id)}
          deleteLabel={t('categories.delete')}
          footer={<CategoryWindows category={category} {...windowActions} />}
        />
      )}
    />
  );
};
