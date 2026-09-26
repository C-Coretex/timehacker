import { createContext } from 'react';

/**
 * Hands typed clock text from `ClockTextInput` (rendered deep inside rc-picker) back to its `ClockStepper`,
 * which decides whether the text is a valid value.
 */
export const ClockTypingContext = createContext<(text: string) => void>(() => {});
