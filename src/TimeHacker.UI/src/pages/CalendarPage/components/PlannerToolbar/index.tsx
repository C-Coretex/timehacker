import type { FC } from 'react';
import { Button, Segmented, Tooltip } from 'antd';
import { LeftOutlined, PlusCircleFilled, RightOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { MagicWandIcon } from 'components/MagicWandIcon';
import type { CalendarView } from 'contexts/CalendarDateContext';
import './styles.css';

interface PlannerToolbarProps {
  title: string;
  view: CalendarView;
  onViewChange: (view: CalendarView) => void;
  onPrevious: () => void;
  onNext: () => void;
  onToday: () => void;
  /** The planner already sits on today, so "Today" has nothing to do. */
  isToday: boolean;
  onReplan: () => void;
  /** False when every visible day is already past — re-planning history would rewrite it. */
  canReplan: boolean;
  busy: boolean;
  onAddTask: () => void;
}

/** The planner's header row: range title, D/3D/W/M switch, paging, re-plan and "Add task". */
export const PlannerToolbar: FC<PlannerToolbarProps> = ({
  title,
  view,
  onViewChange,
  onPrevious,
  onNext,
  onToday,
  isToday,
  onReplan,
  canReplan,
  busy,
  onAddTask,
}) => {
  const { t } = useTranslation();

  const viewOptions = [
    { value: 'day', label: t('calendar.viewShort.day'), title: t('calendar.day') },
    { value: '3day', label: t('calendar.viewShort.threeDay'), title: t('calendar.threeDayView') },
    { value: 'week', label: t('calendar.viewShort.week'), title: t('calendar.week') },
    { value: 'month', label: t('calendar.viewShort.month'), title: t('calendar.month') },
  ];

  return (
    <div className="th-planner-toolbar">
      <h1 className="th-planner-toolbar__title">{title}</h1>

      <div className="th-planner-toolbar__controls">
        <Segmented size="small" options={viewOptions} value={view} onChange={(value) => onViewChange(value as CalendarView)} />
        <div className="th-planner-toolbar__paging">
          <button type="button" aria-label={t('calendar.previous')} onClick={onPrevious}>
            <LeftOutlined />
          </button>
          <button type="button" className="th-planner-toolbar__today" disabled={isToday} onClick={onToday}>
            {t('calendar.today')}
          </button>
          <button type="button" aria-label={t('calendar.next')} onClick={onNext}>
            <RightOutlined />
          </button>
        </div>
      </div>

      <div className="th-planner-toolbar__actions">
        <Tooltip title={t('calendar.replanHint')}>
          <Button
            className="th-planner-toolbar__replan"
            icon={<MagicWandIcon />}
            aria-label={t('calendar.replan')}
            disabled={!canReplan}
            loading={busy}
            onClick={onReplan}
          >
            <span className="th-planner-toolbar__replan-label">{t('calendar.replan')}</span>
          </Button>
        </Tooltip>
        <Button
          className="th-planner-toolbar__add"
          type="primary"
          icon={<PlusCircleFilled />}
          iconPlacement="end"
          aria-label={t('calendar.addTask')}
          onClick={onAddTask}
        >
          <span className="th-planner-toolbar__add-label">{t('calendar.addTask')}</span>
        </Button>
      </div>
    </div>
  );
};
