/**
 * The API represents System.Drawing.Color as a signed ARGB int32 (see ColorJsonConverter), while antd's
 * ColorPicker speaks hex. These bridge the two.
 */

/** ARGB int32 (as sent by the API) to a `#rrggbb` string. */
export function argbToHex(argb: number): string {
  const rgb = argb & 0xffffff;
  return `#${rgb.toString(16).padStart(6, '0')}`;
}

/** `#rrggbb` (or `#rgb`) to a fully opaque ARGB int32. */
export function hexToArgb(hex: string): number {
  const normalized = hex.replace('#', '');
  const expanded =
    normalized.length === 3
      ? normalized
          .split('')
          .map((c) => c + c)
          .join('')
      : normalized;

  const rgb = Number.parseInt(expanded.slice(0, 6), 16);
  // `| 0` keeps the result a signed int32, matching what Color.ToArgb() produces server-side.
  return ((0xff << 24) | rgb) | 0;
}
