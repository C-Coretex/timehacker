import type { FC } from 'react';
import { ClockStepper } from 'components/ClockStepper';
import { LAST_MINUTE_OF_DAY, MINUTE_STEP } from 'utils/minuteStep';

interface DurationFieldProps {
  /** Form.Item-controlled length in minutes. */
  value?: number | null;
  onChange?: (minutes: number | null) => void;
  id?: string;
  /** Adds a clear button, for a duration that may be left empty. */
  optional?: boolean;
}

/** A length of time as H:MM on the digital clock, in 5-minute steps. */
export const DurationField: FC<DurationFieldProps> = ({ value, onChange, id, optional = false }) => (
  <ClockStepper
    id={id}
    minutes={value ?? null}
    onChange={(minutes) => onChange?.(minutes)}
    format="H:mm"
    min={MINUTE_STEP}
    max={LAST_MINUTE_OF_DAY}
    emptyStart={() => 0}
    allowClear={optional}
  />
);
