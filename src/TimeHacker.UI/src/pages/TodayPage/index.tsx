import { useCallback, useEffect, useMemo } from 'react';
import type { FC } from 'react';
import { Alert, Button, Tooltip } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { EventDetailModal } from 'components/EventDetailModal';
import { ListCardList } from 'components/ListCard';
import { MagicWandIcon } from 'components/MagicWandIcon';
import { PageHeader } from 'components/PageHeader';
import { UnifiedTaskFormModal } from 'components/UnifiedTaskFormModal';
import { useCalendarTasks } from 'hooks/useCalendarTasks';
import { useEventDetails } from 'hooks/useEventDetails';
import { useTaskCategories } from 'hooks/useTaskCategories';
import { useTaskComposer } from 'hooks/useTaskComposer';
import { attachCategoriesToEvents, isTaskEvent } from 'utils/calendarUtils';
import { summarizeDay } from 'utils/daySummary';
import { TodayTaskRow } from './components/TodayTaskRow';
import './styles.css';

/**
 * "Tasks for today": today's plan as a list, with re-plan and add-task shortcuts. Read-only — the API does not
 * track completion yet, so rows open details rather than tick off.
 */
export const TodayPage: FC = () => {
  const { t } = useTranslation();
  const today = useMemo(() => [dayjs().startOf('day').toDate()], []);
  const { events, loading, error, fetchTasks, refresh } = useCalendarTasks();
  const { categoriesByTaskId, fetchTaskCategories } = useTaskCategories();
  const details = useEventDetails();
  const reload = useCallback(() => Promise.all([fetchTasks(today), fetchTaskCategories()]), [fetchTasks, fetchTaskCategories, today]);
  const composer = useTaskComposer(reload);

  useEffect(() => {
    void fetchTasks(today);
  }, [fetchTasks, today]);

  const tasks = useMemo(
    () =>
      attachCategoriesToEvents(events, categoriesByTaskId)
        .filter(isTaskEvent)
        .sort((a, b) => a.start.getTime() - b.start.getTime()),
    [events, categoriesByTaskId]
  );
  const now = new Date();
  const summary = summarizeDay(tasks, today[0], now);

  return (
    <div className="th-today">
      <PageHeader
        title={t('today.title')}
        actions={
          <>
            <Tooltip title={t('calendar.replanHint')}>
              <Button
                type="primary"
                shape="circle"
                icon={<MagicWandIcon />}
                aria-label={t('calendar.replan')}
                loading={loading}
                onClick={() => void refresh(today)}
              />
            </Tooltip>
            <Button type="primary" shape="circle" icon={<EditOutlined />} aria-label={t('calendar.addTask')} onClick={() => composer.open()} />
          </>
        }
      >
        <span className="th-today__chip">{t('today.planned', { count: summary.planned })}</span>
        <span className="th-today__chip">{t('today.remaining', { count: summary.remaining })}</span>
      </PageHeader>

      {error && <Alert type="error" title={error} showIcon className="th-today__error" />}

      <ListCardList
        items={tasks}
        loading={loading}
        emptyText={t('today.empty')}
        renderItem={(task) => <TodayTaskRow event={task} isPast={task.end < now} onOpen={details.open} />}
      />

      <EventDetailModal
        open={details.isOpen}
        onClose={details.close}
        event={details.event}
        scheduleEntity={details.scheduleEntity}
        loadingSchedule={details.loadingSchedule}
      />

      <UnifiedTaskFormModal
        open={composer.isOpen}
        prefill={composer.prefill}
        onCancel={composer.close}
        onSaveFixed={composer.saveFixed}
        onSaveDynamic={composer.saveDynamic}
      />
    </div>
  );
};
