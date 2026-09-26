import { forwardRef, useContext, useState } from 'react';
import type { InputHTMLAttributes } from 'react';
import { sanitizeClockText } from 'utils/clockText';
import { ClockTypingContext } from './clockTypingContext';

/**
 * The text box inside ClockStepper's TimePicker, swapped in through rc-picker's `components.input`.
 * rc-picker only accepts text in the display format; this keeps a draft instead (digits and one colon) and
 * hands it over on Enter or blur. rc-picker never sees the typing, so its own Enter/blur handling cannot
 * re-submit the stale value over the one just typed.
 */
export const ClockTextInput = forwardRef<HTMLInputElement, InputHTMLAttributes<HTMLInputElement>>(
  ({ value, onBlur, onKeyDown, ...rest }, ref) => {
    const commitTyped = useContext(ClockTypingContext);
    const [draft, setDraft] = useState<string | null>(null);

    const commit = () => {
      if (draft !== null) commitTyped(draft);
      setDraft(null);
    };

    return (
      <input
        {...rest}
        ref={ref}
        value={draft ?? value ?? ''}
        inputMode="numeric"
        maxLength={5}
        onChange={(event) => setDraft(sanitizeClockText(event.target.value))}
        onBlur={(event) => {
          commit();
          onBlur?.(event);
        }}
        onKeyDown={(event) => {
          if (event.key === 'Enter') {
            // Blurring commits and lets rc-picker close its panel the usual way.
            event.currentTarget.blur();
            return;
          }
          if (event.key === 'Escape') setDraft(null);
          onKeyDown?.(event);
        }}
      />
    );
  }
);

ClockTextInput.displayName = 'ClockTextInput';
