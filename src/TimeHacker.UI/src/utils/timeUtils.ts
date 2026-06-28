/** Format date as YYYY-MM-DD (ISO 8601) */
export function formatDateIso(d: Date): string {
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

/**
 * Parse a C# TimeSpan string to minutes from midnight.
 * Handles: "HH:mm:ss", "d.HH:mm:ss", "HH:mm:ss.fffffff", ISO 8601 "PT1H30M"
 */
export function parseTimeToMinutes(value: string): number {
  if (!value) return 0;

  const isoMatch = value.match(/^PT(?:(\d+)H)?(?:(\d+)M)?(?:(\d+(?:\.\d+)?)S)?$/i);
  if (isoMatch) {
    const h = parseInt(isoMatch[1] ?? '0', 10);
    const m = parseInt(isoMatch[2] ?? '0', 10);
    const s = parseFloat(isoMatch[3] ?? '0');
    return h * 60 + m + s / 60;
  }

  // "d.HH:mm:ss" — dot appears before the first colon
  const daysMatch = value.match(/^(\d+)\.(\d+):(\d+):(\d+)/);
  if (daysMatch) {
    const days = parseInt(daysMatch[1], 10);
    const hours = parseInt(daysMatch[2], 10);
    const mins = parseInt(daysMatch[3], 10);
    const secs = parseInt(daysMatch[4], 10);
    return days * 24 * 60 + hours * 60 + mins + secs / 60;
  }

  const timeMatch = value.match(/^(\d+):(\d+):(\d+)/);
  if (timeMatch) {
    const hours = parseInt(timeMatch[1], 10);
    const mins = parseInt(timeMatch[2], 10);
    const secs = parseInt(timeMatch[3], 10);
    return hours * 60 + mins + secs / 60;
  }

  return 0;
}

/**
 * Build a Date from a UTC time-of-day (minutes past UTC midnight) on the given calendar date.
 *
 * The timeline `timeRange` values arrive from the backend as bare time-of-day strings derived from
 * the UTC `StartTimestamp`/`EndTimestamp` (`DateTime.TimeOfDay`), so they carry no timezone marker.
 * We anchor them to UTC midnight of the calendar date and return a Date that renders in the user's
 * local timezone — otherwise the UTC time-of-day would be shown verbatim as local time, shifting
 * every event by the local UTC offset.
 */
export function utcMinutesToDate(date: Date, minutesFromMidnight: number): Date {
  const utcMidnight = Date.UTC(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0, 0);
  return new Date(utcMidnight + minutesFromMidnight * 60_000);
}

/** Convert a duration in minutes to an HH:mm:ss TimeSpan string */
export function minutesToTimeSpan(minutes: number): string {
  const h = Math.floor(minutes / 60);
  const m = minutes % 60;
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${pad(h)}:${pad(m)}:00`;
}

/** Parse an HH:mm:ss TimeSpan string to a duration in minutes */
export function timeSpanToMinutes(value: string): number {
  if (!value) return 0;
  const parts = value.split(/[.:]/).map(Number);
  if (parts.length >= 3) {
    const [h, m] = parts;
    return (h ?? 0) * 60 + (m ?? 0);
  }
  return 0;
}
