import type { ReactNode } from 'react';
import './styles.css';

export interface PillOption<T> {
  value: T;
  label: ReactNode;
}

interface PillGroupProps<T> {
  options: PillOption<T>[];
  value?: T;
  onChange?: (value: T) => void;
  id?: string;
  'aria-label'?: string;
}

/**
 * One choice from a wrapping row of pills — the design's chips, for when there are too many options for a
 * Segmented to fit. Works as a Form.Item child.
 */
export function PillGroup<T extends string | number>({ options, value, onChange, id, 'aria-label': ariaLabel }: PillGroupProps<T>) {
  return (
    <div className="th-pill-group" role="radiogroup" id={id} aria-label={ariaLabel}>
      {options.map((option) => (
        <button
          key={option.value}
          type="button"
          role="radio"
          aria-checked={option.value === value}
          className={`th-pill${option.value === value ? ' is-selected' : ''}`}
          onClick={() => onChange?.(option.value)}
        >
          {option.label}
        </button>
      ))}
    </div>
  );
}
