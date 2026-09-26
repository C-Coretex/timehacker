import type { FC } from 'react';
import { Form } from 'antd';
import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import { RepeatingEntityTypeEnum } from 'api/types';
import { PillGroup } from 'components/PillGroup';
import { EndsFields } from './EndsFields';
import { REPEAT_NONE, getFrequencyOptions } from './constants';
import type { FrequencyChoice } from './constants';
import { RecurrenceTypeFields } from './RecurrenceTypeFields';
import './styles.css';

interface ScheduleFormSectionProps {
  /**
   * The day the entity being created already occupies. The server anchors the recurrence there and
   * only ever walks forward, so specific dates at or before it (or before today) can never produce an
   * occurrence and are greyed out.
   */
  anchorDate?: Dayjs;
}

interface FrequencyPillsProps {
  value?: RepeatingEntityTypeEnum;
  onChange?: (value: RepeatingEntityTypeEnum | undefined) => void;
  id?: string;
}

/** The recurrence type as pills, "Does not repeat" first. Form value: the type, or undefined for none. */
const FrequencyPills: FC<FrequencyPillsProps> = ({ value, onChange, id }) => {
  const { t } = useTranslation();

  return (
    <PillGroup<FrequencyChoice>
      id={id}
      aria-label={t('taskForm.repeats')}
      options={getFrequencyOptions(t)}
      value={value ?? REPEAT_NONE}
      onChange={(choice) => onChange?.(choice === REPEAT_NONE ? undefined : choice)}
    />
  );
};

/**
 * The recurrence fields (type, its parameters, when it ends). They sit in the host form, so
 * `buildSchedulePayload` reads them straight from its values; an empty `scheduleType` means no recurrence.
 */
export const ScheduleFormSection: FC<ScheduleFormSectionProps> = ({ anchorDate }) => {
  const scheduleType = Form.useWatch<RepeatingEntityTypeEnum | undefined>('scheduleType');
  const anchor = anchorDate ?? dayjs();
  const onceFloor = anchor.isAfter(dayjs(), 'day') ? anchor : dayjs();

  return (
    <div className="th-schedule-fields">
      <Form.Item name="scheduleType" noStyle>
        <FrequencyPills />
      </Form.Item>

      {scheduleType != null && (
        <div className="th-schedule-fields__detail">
          <RecurrenceTypeFields scheduleType={scheduleType} anchor={anchor} onceFloor={onceFloor} />
          {/* An explicit list of dates is already bounded, so the server derives its own end date. */}
          {scheduleType !== RepeatingEntityTypeEnum.OnceRepeatingEntity && <EndsFields anchor={anchor} />}
        </div>
      )}
    </div>
  );
};
