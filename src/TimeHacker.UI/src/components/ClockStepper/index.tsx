import type { FC } from 'react';
import { TimePicker } from 'antd';
import { ClockCircleOutlined, MinusOutlined, PlusOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { parseClockText } from 'utils/clockText';
import type { ClockText } from 'utils/clockText';
import { MINUTE_STEP, stepFrom } from 'utils/minuteStep';
import { ClockTextInput } from './ClockTextInput';
import { ClockTypingContext } from './clockTypingContext';
import './styles.css';

interface ClockStepperProps {
  /** Minutes past midnight for a time of day, or the length of a duration. */
  minutes: number | null;
  onChange: (minutes: number | null) => void;
  format: string;
  use12Hours?: boolean;
  min: number;
  max: number;
  /** Where the first − / + press starts from while the field is still empty. */
  emptyStart: () => number;
  /** Turns typed hours and minutes into this field's minutes; null rejects them. Defaults to H × 60 + M. */
  readTyped?: (typed: ClockText) => number | null;
  allowClear?: boolean;
  id?: string;
}

// Module scope: rc-picker rebuilds its context when the components object changes identity.
const PICKER_COMPONENTS = { input: ClockTextInput };
const POPUP_CLASS_NAMES = { popup: { root: 'th-clock-popup' } };

const onClock = (minutes: number): Dayjs => dayjs().startOf('day').add(minutes, 'minute');
const minutesOf = (time: Dayjs): number => time.hour() * 60 + time.minute();
const plainReading = ({ hours, minutes }: ClockText) => hours * 60 + minutes;

/**
 * The design's 7-segment clock between − and + buttons that move it by five minutes. The clock icon opens a
 * picker in 5-minute steps; typing takes up to four digits ("1422", "12", "12:11") and keeps the exact minutes.
 */
export const ClockStepper: FC<ClockStepperProps> = ({
  minutes,
  onChange,
  format,
  use12Hours,
  min,
  max,
  emptyStart,
  readTyped = plainReading,
  allowClear = false,
  id,
}) => {
  const { t } = useTranslation();
  const clamp = (value: number) => Math.min(max, Math.max(min, value));
  const step = (direction: 1 | -1) => onChange(clamp(stepFrom(minutes ?? emptyStart(), direction)));

  // Anything unreadable or out of range is dropped, and the field shows its previous value again.
  const commitTyped = (text: string) => {
    if (text === '') {
      if (allowClear) onChange(null);
      return;
    }
    const typed = parseClockText(text);
    const next = typed && readTyped(typed);
    if (next != null && next >= min && next <= max) onChange(next);
  };

  return (
    <span className="th-clock-stepper">
      <button
        type="button"
        className="th-clock-stepper__step"
        aria-label={t('taskForm.minusMinutes', { step: MINUTE_STEP })}
        disabled={minutes != null && minutes <= min}
        onClick={() => step(-1)}
      >
        <MinusOutlined />
      </button>
      <ClockTypingContext.Provider value={commitTyped}>
        <TimePicker
          id={id}
          className="th-clock-stepper__picker"
          classNames={POPUP_CLASS_NAMES}
          components={PICKER_COMPONENTS}
          variant="borderless"
          value={minutes == null ? null : onClock(minutes)}
          onChange={(time) => onChange(time ? clamp(minutesOf(time)) : null)}
          format={format}
          use12Hours={use12Hours}
          minuteStep={MINUTE_STEP}
          placeholder="--:--"
          suffixIcon={<ClockCircleOutlined />}
          allowClear={allowClear}
          needConfirm={false}
          showNow={false}
        />
      </ClockTypingContext.Provider>
      <button
        type="button"
        className="th-clock-stepper__step"
        aria-label={t('taskForm.plusMinutes', { step: MINUTE_STEP })}
        disabled={minutes != null && minutes >= max}
        onClick={() => step(1)}
      >
        <PlusOutlined />
      </button>
    </span>
  );
};
