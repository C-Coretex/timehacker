import { useContext } from 'react';
import type { FC, ReactNode, SyntheticEvent } from 'react';
import { PlusOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { useTranslation } from 'react-i18next';
import { useSettings } from 'contexts/SettingsContext';
import { QuickAddContext } from '../../quickAddContext';
import './styles.css';

// rbc starts slot selection from a `document` mousedown listener; stopping the press here keeps a click on
// the button from also starting a range selection underneath it.
const stopSlotSelection = (event: SyntheticEvent) => event.stopPropagation();

// Optional because @types/react-big-calendar types the wrapper as taking no props; rbc does pass both.
interface QuickAddSlotProps {
  value?: Date;
  children?: ReactNode;
}

/**
 * rbc's `timeSlotWrapper`: the first slot of every hour also carries that hour's "+" button, which CSS shows
 * while the hour is hovered. It is kept out of the tab order — "Add task" in the toolbar is the keyboard path.
 * The time gutter uses the same wrapper; the CSS shows the button only inside day columns.
 */
export const QuickAddSlot: FC<QuickAddSlotProps> = ({ value, children }) => {
  const { t } = useTranslation();
  const { timeDisplayFormat } = useSettings();
  const onQuickAdd = useContext(QuickAddContext);

  if (!onQuickAdd || !value || value.getMinutes() !== 0) return <>{children}</>;

  return (
    <>
      {children}
      <button
        type="button"
        tabIndex={-1}
        className="th-quick-add"
        aria-label={t('calendar.addTaskAt', { time: dayjs(value).format(timeDisplayFormat) })}
        onMouseDown={stopSlotSelection}
        onTouchStart={stopSlotSelection}
        onClick={() => onQuickAdd(value)}
      >
        <PlusOutlined />
      </button>
    </>
  );
};
