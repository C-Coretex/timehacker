import type { FC } from 'react';
import { Segmented } from 'antd';
import { CheckOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import type { TaskTab } from './types';

interface TaskTypeToggleProps {
  value: TaskTab;
  onChange: (type: TaskTab) => void;
  /** An existing task cannot change type. */
  disabled: boolean;
}

/** The design's "✓ Fixed | Dynamic" pill switch, with the one-line meaning of the chosen type under it. */
export const TaskTypeToggle: FC<TaskTypeToggleProps> = ({ value, onChange, disabled }) => {
  const { t } = useTranslation();
  const label = (type: TaskTab, text: string) => (
    <span className="th-task-type__option">
      {value === type && <CheckOutlined />}
      {text}
    </span>
  );

  return (
    <div className="th-task-type">
      <Segmented<TaskTab>
        shape="round"
        value={value}
        disabled={disabled}
        onChange={onChange}
        options={[
          { value: 'fixed', label: label('fixed', t('taskForm.fixedTask')) },
          { value: 'dynamic', label: label('dynamic', t('taskForm.dynamicTask')) },
        ]}
      />
      <p className="th-task-type__hint">{value === 'fixed' ? t('taskForm.fixedHint') : t('taskForm.dynamicHint')}</p>
    </div>
  );
};
