/** − / + and the picker's minute column move in 5-minute steps; a typed value keeps its exact minutes. */
export const MINUTE_STEP = 5;

/** 23:59 — the latest time of day a clock can hold (typed minutes are exact), and its longest duration. */
export const LAST_MINUTE_OF_DAY = 24 * 60 - 1;

/** The nearest step — where stepping starts when a field begins empty at "now". */
export const snapToStep = (minutes: number): number => Math.round(minutes / MINUTE_STEP) * MINUTE_STEP;

/** Five minutes later or earlier, from whatever the value is: a typed 14:22 steps to 14:27, not 14:25. */
export const stepFrom = (minutes: number, direction: 1 | -1): number => minutes + direction * MINUTE_STEP;
