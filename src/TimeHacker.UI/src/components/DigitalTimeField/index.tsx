import type { FC } from 'react';
import { Segmented } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { ClockStepper } from 'components/ClockStepper';
import { useSettings } from 'contexts/SettingsContext';
import type { ClockText } from 'utils/clockText';
import { LAST_MINUTE_OF_DAY, snapToStep } from 'utils/minuteStep';
import './styles.css';

interface DigitalTimeFieldProps {
  /** Form.Item-controlled time of day. */
  value?: Dayjs | null;
  onChange?: (value: Dayjs | null) => void;
  id?: string;
}

type Meridiem = 'am' | 'pm';

const minutesOfDay = (time: Dayjs) => time.hour() * 60 + time.minute();

/** A time of day on the 7-segment clock, with an AM/PM switch when the 12-hour setting is on. */
export const DigitalTimeField: FC<DigitalTimeFieldProps> = ({ value, onChange, id }) => {
  const { t } = useTranslation();
  const { timeFormat } = useSettings();
  const is12Hour = timeFormat === '12h';
  const meridiem: Meridiem = value && value.hour() >= 12 ? 'pm' : 'am';

  // Only the time of day is ours to change; the date part belongs to whoever set the value.
  const setMinutes = (minutes: number | null) =>
    onChange?.(
      minutes == null
        ? null
        : (value ?? dayjs())
            .hour(Math.floor(minutes / 60))
            .minute(minutes % 60)
            .second(0)
            .millisecond(0)
    );

  // On a 12-hour clock "2:30" keeps the AM/PM already chosen; 13–23 can only mean the afternoon.
  const readTyped = ({ hours, minutes }: ClockText) => {
    if (hours > 23) return null;
    const hour24 = is12Hour && hours >= 1 && hours <= 12 ? (hours % 12) + (meridiem === 'pm' ? 12 : 0) : hours;
    return hour24 * 60 + minutes;
  };

  const switchMeridiem = (next: Meridiem) => {
    if (value) onChange?.(value.hour((value.hour() % 12) + (next === 'pm' ? 12 : 0)));
  };

  return (
    <span className="th-digital-time">
      <ClockStepper
        id={id}
        minutes={value ? minutesOfDay(value) : null}
        onChange={setMinutes}
        format={is12Hour ? 'hh:mm' : 'HH:mm'}
        use12Hours={is12Hour}
        min={0}
        max={LAST_MINUTE_OF_DAY}
        emptyStart={() => snapToStep(minutesOfDay(dayjs()))}
        readTyped={readTyped}
      />
      {is12Hour && (
        <Segmented<Meridiem>
          size="small"
          className="th-digital-time__meridiem"
          value={meridiem}
          disabled={!value}
          onChange={switchMeridiem}
          options={[
            { label: t('taskForm.am'), value: 'am' },
            { label: t('taskForm.pm'), value: 'pm' },
          ]}
        />
      )}
    </span>
  );
};
