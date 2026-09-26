import type { FC } from 'react';
import { Slider } from 'antd';
import type { SliderSingleProps } from 'antd';
import { useTranslation } from 'react-i18next';
import { PRIORITY_DEFAULT, PRIORITY_HIGHEST, PRIORITY_LOWEST, priorityLabel } from 'utils/priority';
import './styles.css';

type PrioritySliderProps = Pick<SliderSingleProps, 'value' | 'onChange' | 'disabled'>;

/**
 * The 1–5 priority picker. Reversed so the design's left-to-right Lowest → Highest reading holds even
 * though 1 is the highest priority.
 */
export const PrioritySlider: FC<PrioritySliderProps> = (props) => {
  const { t } = useTranslation();

  const marks = {
    [PRIORITY_LOWEST]: priorityLabel(PRIORITY_LOWEST, t),
    [PRIORITY_LOWEST - 1]: ' ',
    [PRIORITY_DEFAULT]: priorityLabel(PRIORITY_DEFAULT, t),
    [PRIORITY_HIGHEST + 1]: ' ',
    [PRIORITY_HIGHEST]: priorityLabel(PRIORITY_HIGHEST, t),
  };

  return (
    <Slider
      {...props}
      className="th-priority-slider"
      min={PRIORITY_HIGHEST}
      max={PRIORITY_LOWEST}
      step={1}
      reverse
      marks={marks}
      tooltip={{ formatter: (value) => (value == null ? null : priorityLabel(value, t)) }}
    />
  );
};
