import type { FC } from 'react';
import { Form } from 'antd';
import type { Dayjs } from 'dayjs';
import type { ScheduleEntityReturnModel } from 'api/types';
import { CalendarCard } from 'components/CalendarCard';
import { RepeatPicker } from 'components/RepeatPicker';
import { ScheduleSummary } from 'components/ScheduleSummary';
import { TimeRangeFields } from 'components/TimeRangeFields';

interface WhenFieldsProps {
  /**
   * Editing: recurrences are attached at creation only, so an existing one is shown read-only instead of
   * the repeat picker.
   */
  isEdit: boolean;
  scheduleEntity?: ScheduleEntityReturnModel | null;
}

/**
 * "When" for anything placed on a day — a fixed task or a category window: the day on the design's
 * calendar card, with its time range and repeat rule in the card's footer. Fields: `date`, `startTime`,
 * `endTime`, plus the recurrence fields while creating.
 */
export const WhenFields: FC<WhenFieldsProps> = ({ isEdit, scheduleEntity }) => {
  const anchorDate = Form.useWatch<Dayjs | undefined>('date');

  return (
    <Form.Item name="date" noStyle>
      <CalendarCard
        footer={
          <>
            {isEdit ? <ScheduleSummary scheduleEntity={scheduleEntity ?? null} /> : <RepeatPicker anchorDate={anchorDate} />}
            <TimeRangeFields />
          </>
        }
      />
    </Form.Item>
  );
};
