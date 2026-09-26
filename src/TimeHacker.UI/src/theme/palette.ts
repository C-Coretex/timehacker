/**
 * The design palette — the single source of every colour the UI paints. Components never hardcode a hex:
 * antd reads these through `buildAntdTheme`, plain CSS through the `--th-*` variables `installCssVariables`
 * derives from them.
 */
export interface Palette {
  primary: string;
  primaryHover: string;
  /** Tint behind the quick-add slot and other magenta affordances. */
  primarySoft: string;
  onPrimary: string;
  /** Start of the purple → magenta brand gradient (logo, accents). */
  brandPurple: string;
  indigo: string;
  navy: string;
  lavender100: string;
  lavender200: string;
  text: string;
  textMuted: string;
  /** The whole app's backdrop, sidebar included, under the cursor glow. */
  page: string;
  /** Everything that is a component (cards, tables, the calendar grid) sits on this. */
  panel: string;
  border: string;
  grid: string;
  gridSoft: string;
  sidebarText: string;
  navActiveBg: string;
  navActiveText: string;
  gradientPink: string;
  gradientLavender: string;
  eventFixed: string;
  eventFixedSoft: string;
  eventDynamic: string;
  eventDynamicSoft: string;
  /** Phone day-view rows, which the design paints stronger than the desktop cards. */
  eventFixedRow: string;
  eventDynamicRow: string;
  onEvent: string;
  /** Text on the orange dynamic block, where white would not read. */
  onEventDynamic: string;
  todayCircle: string;
  slider: string;
  shadow: string;
  /** How strongly a category band tints its window, as a `color-mix` percentage. */
  bandAlpha: string;
  priorityHighest: string;
  priorityHigh: string;
  priorityNormal: string;
  priorityLow: string;
  priorityLowest: string;
}

export const lightPalette: Palette = {
  primary: '#D9268F',
  primaryHover: '#C01F7E',
  primarySoft: '#FCE4F1',
  onPrimary: '#FFFFFF',
  brandPurple: '#6C47C9',
  indigo: '#3E3C9C',
  navy: '#2E3192',
  lavender100: '#EEEDF8',
  lavender200: '#DCDAEE',
  text: '#1F1D36',
  textMuted: '#6B6A8A',
  page: '#FBF6FA',
  panel: '#FFFFFF',
  border: '#E4E2F0',
  grid: '#E6E4F2',
  gridSoft: '#F3F2F9',
  sidebarText: '#3E3C9C',
  navActiveBg: '#DCDAEE',
  navActiveText: '#2E3192',
  gradientPink: '#F7C4E3',
  gradientLavender: '#D8CCF6',
  eventFixed: '#6C47C9',
  eventFixedSoft: '#EEEBFB',
  eventDynamic: '#F5A623',
  eventDynamicSoft: '#FFF3D6',
  eventFixedRow: '#D9D2F0',
  eventDynamicRow: '#FBD54A',
  onEvent: '#FFFFFF',
  onEventDynamic: '#2B1D00',
  todayCircle: '#6F70B8',
  slider: '#7B7BC4',
  shadow: 'rgba(46, 49, 146, 0.12)',
  bandAlpha: '16%',
  priorityHighest: '#E5484D',
  priorityHigh: '#F76B15',
  priorityNormal: '#7B7BC4',
  priorityLow: '#12A594',
  priorityLowest: '#8E8CA8',
};

// Only the sidebar is specified by the design in dark mode; the rest follows the same roles.
export const darkPalette: Palette = {
  primary: '#E0439F',
  primaryHover: '#EA64B1',
  primarySoft: '#3A1D30',
  onPrimary: '#FFFFFF',
  brandPurple: '#8B67E6',
  indigo: '#AFADF2',
  navy: '#6E71E6',
  lavender100: '#26253A',
  lavender200: '#35334F',
  text: '#ECEBF5',
  textMuted: '#9C9BB5',
  page: '#121214',
  panel: '#1C1C21',
  border: '#2E2D3D',
  grid: '#2B2A3A',
  gridSoft: '#222130',
  sidebarText: '#CFCDE8',
  navActiveBg: '#D9268F',
  navActiveText: '#FFFFFF',
  gradientPink: '#5A2449',
  gradientLavender: '#342B66',
  eventFixed: '#7D5BDB',
  eventFixedSoft: '#2B2445',
  eventDynamic: '#F5A623',
  eventDynamicSoft: '#3A2F1A',
  eventFixedRow: '#3A3163',
  eventDynamicRow: '#5E4B12',
  onEvent: '#FFFFFF',
  onEventDynamic: '#2B1D00',
  todayCircle: '#7C7DD0',
  slider: '#6C6CB8',
  shadow: 'rgba(0, 0, 0, 0.45)',
  bandAlpha: '24%',
  priorityHighest: '#FF6369',
  priorityHigh: '#FF8B3E',
  priorityNormal: '#9B9BE0',
  priorityLow: '#3DD6B8',
  priorityLowest: '#8E8CA8',
};

/** Suggested category colours (stored data, so theme-independent); the first is the default. */
export const categoryColorPresets = [
  '#D9268F',
  '#6C47C9',
  '#3A9BDC',
  '#1DBF8E',
  '#F5A623',
  '#E03C31',
  '#7B7BC4',
  '#12A594',
] as const;

export const radii = { control: 6, card: 10, pill: 999 } as const;

export const fonts = {
  sans: "'Inter Variable', Inter, system-ui, -apple-system, 'Segoe UI', sans-serif",
  digital: "'DSEG7 Classic', 'Inter Variable', monospace",
} as const;

export const paletteFor = (isDark: boolean): Palette => (isDark ? darkPalette : lightPalette);
