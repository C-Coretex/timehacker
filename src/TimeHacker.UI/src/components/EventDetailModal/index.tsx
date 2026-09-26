import type { CSSProperties, FC } from 'react';
import { Modal } from 'antd';
import { ClockCircleOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import type { ScheduleEntityReturnModel } from 'api/types';
import { CategoryTags } from 'components/CategoryTags';
import { PriorityTag } from 'components/PriorityTag';
import { ScheduleSummary } from 'components/ScheduleSummary';
import { useSettings } from 'contexts/SettingsContext';
import { isTaskEvent } from 'utils/calendarUtils';
import type { CalendarEvent } from 'utils/calendarUtils';
import { argbToHex } from 'utils/colorArgb';
import './styles.css';

interface EventDetailModalProps {
  open: boolean;
  onClose: () => void;
  event: CalendarEvent | null;
  /** A fixed task's recurrence, loaded when the dialog opens. */
  scheduleEntity: ScheduleEntityReturnModel | null;
  loadingSchedule: boolean;
}

const accentOf = (event: CalendarEvent): string | undefined => {
  const resource = event.resource;
  if (resource?.type === 'category') return argbToHex(resource.category.color);
  const [firstCategory] = resource?.categories ?? [];
  return firstCategory ? argbToHex(firstCategory.color) : undefined;
};

/** Read-only details of a planner item: a task (type, priority, categories, recurrence) or a category band. */
export const EventDetailModal: FC<EventDetailModalProps> = ({ open, onClose, event, scheduleEntity, loadingSchedule }) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();
  const resource = event?.resource;
  const task = event && isTaskEvent(event) ? event.resource : undefined;
  const typeLabel =
    resource?.type === 'category' ? t('calendar.category') : resource?.type === 'dynamic' ? t('calendar.dynamic') : t('calendar.fixed');

  return (
    <Modal open={open} onCancel={onClose} footer={null} width={520} title={null}>
      {event && (
        <div
          className={`th-event-detail th-event-detail--${resource?.type ?? 'fixed'}`}
          style={{ '--th-detail-accent': accentOf(event) } as CSSProperties}
        >
          <div className="th-event-detail__badges">
            <span className="th-event-detail__type">{typeLabel}</span>
            {task && <PriorityTag priority={task.task.priority} />}
          </div>

          <h2 className="th-event-detail__title">{event.title}</h2>

          {/* A category can contribute several bands to one day; this says which window this is. */}
          {resource?.type === 'category' && resource.scheduleDescription && (
            <p className="th-event-detail__note">{resource.scheduleDescription}</p>
          )}
          {event.description && <p className="th-event-detail__description">{event.description}</p>}

          <div className="th-event-detail__time">
            <ClockCircleOutlined />
            {dayjs(event.start).format('ddd, D MMM')} · {dayjs(event.start).format(timeDisplayFormat)} →{' '}
            {dayjs(event.end).format(timeDisplayFormat)}
          </div>

          {task && task.categories.length > 0 && (
            <div className="th-event-detail__categories">
              <CategoryTags categories={task.categories} />
            </div>
          )}

          {resource?.type === 'fixed' && <ScheduleSummary scheduleEntity={scheduleEntity} loading={loadingSchedule} />}
        </div>
      )}
    </Modal>
  );
};
