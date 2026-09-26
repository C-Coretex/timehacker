import { useState } from 'react';
import type { FC } from 'react';
import { Form } from 'antd';
import { SyncOutlined } from '@ant-design/icons';
import type { Dayjs } from 'dayjs';
import { useTranslation } from 'react-i18next';
import type { RepeatingEntityTypeEnum } from 'api/types';
import { ScheduleFormSection } from 'components/ScheduleFormSection';
import { buildSchedulePayload } from 'utils/buildSchedulePayload';
import { describeRecurrence } from 'utils/describeRecurrence';
import './styles.css';

/**
 * The design's "⟳ every day" link: it summarises the recurrence being built and opens the panel that edits it.
 * The panel stays open while a recurrence is chosen, so its fields stay registered (and submitted) and a
 * validation error in them can never be hidden.
 */
export const RepeatPicker: FC<{ anchorDate?: Dayjs }> = ({ anchorDate }) => {
  const { t } = useTranslation();
  const form = Form.useFormInstance();
  const [expanded, setExpanded] = useState(false);
  const scheduleType = Form.useWatch<RepeatingEntityTypeEnum | undefined>('scheduleType', { form, preserve: true });
  const payload = Form.useWatch((values) => buildSchedulePayload(values), { form, preserve: true });
  const repeating = scheduleType != null;
  const summary = payload ? describeRecurrence(payload.repeatingEntityType, t) : t('taskForm.doesNotRepeat');
  const panelOpen = expanded || repeating;

  return (
    <div className="th-repeat">
      <button
        type="button"
        className="th-repeat__trigger"
        aria-expanded={panelOpen}
        disabled={repeating}
        onClick={() => setExpanded((open) => !open)}
      >
        <SyncOutlined />
        {summary}
      </button>

      {panelOpen && (
        <div className="th-repeat__panel">
          <ScheduleFormSection anchorDate={anchorDate} />
        </div>
      )}
    </div>
  );
};
