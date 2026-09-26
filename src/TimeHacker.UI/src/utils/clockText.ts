export interface ClockText {
  hours: number;
  minutes: number;
}

/** What a clock accepts while typing: at most four digits, with one optional colon ("1422", "9:05"). */
export function sanitizeClockText(text: string): string {
  const cleaned = text.replace(/[^\d:]/g, '');
  const colon = cleaned.indexOf(':');
  if (colon === -1) return cleaned.slice(0, 4);
  const hours = cleaned.slice(0, colon).slice(0, 2);
  const minutes = cleaned.slice(colon + 1).replace(/:/g, '').slice(0, 2);
  return `${hours}:${minutes}`;
}

/**
 * Reads what was typed into a clock: "1422" → 14:22, "930" → 9:30, "12" → 12:00, "12:11", "9:5" → 9:05.
 * Minutes must be 0–59; hours are left unchecked, since only the caller knows whether this is a time of day
 * or a duration.
 */
export function parseClockText(text: string): ClockText | null {
  let hours: string;
  let minutes: string;
  if (text.includes(':')) {
    [hours, minutes] = text.split(':');
  } else if (text.length <= 2) {
    [hours, minutes] = [text, ''];
  } else {
    [hours, minutes] = [text.slice(0, -2), text.slice(-2)];
  }

  if (!/^\d{1,2}$/.test(hours) || !/^\d{0,2}$/.test(minutes)) return null;
  const minuteValue = minutes === '' ? 0 : Number(minutes);
  return minuteValue > 59 ? null : { hours: Number(hours), minutes: minuteValue };
}
