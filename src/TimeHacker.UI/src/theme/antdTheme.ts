import { theme } from 'antd';
import type { ThemeConfig } from 'antd';
import { fonts, paletteFor, radii } from './palette';

/** Maps the design palette onto antd's seed and component tokens for the given mode. */
export function buildAntdTheme(isDark: boolean): ThemeConfig {
  const palette = paletteFor(isDark);
  // Date cells, tabs and similar selections are navy in the design; magenta is reserved for actions.
  const navySelection = { colorPrimary: palette.navy, algorithm: true };

  return {
    algorithm: isDark ? theme.darkAlgorithm : theme.defaultAlgorithm,
    token: {
      colorPrimary: palette.primary,
      colorLink: palette.indigo,
      colorLinkHover: palette.navy,
      colorTextBase: palette.text,
      colorBgLayout: palette.page,
      colorBgContainer: palette.panel,
      colorBorderSecondary: palette.border,
      fontFamily: fonts.sans,
      borderRadius: radii.control,
      borderRadiusLG: radii.card,
    },
    components: {
      Button: { primaryShadow: 'none', fontWeight: 600 },
      Form: { labelColor: palette.indigo, labelFontSize: 13, verticalLabelPadding: '0 0 4px' },
      Segmented: {
        trackBg: palette.lavender100,
        trackPadding: 3,
        itemColor: palette.indigo,
        itemHoverColor: palette.navy,
        itemHoverBg: palette.lavender200,
        itemActiveBg: palette.lavender200,
        itemSelectedBg: palette.panel,
        itemSelectedColor: palette.navy,
      },
      Slider: {
        railSize: 10,
        handleSize: 10,
        handleSizeHover: 12,
        dotSize: 8,
        railBg: palette.slider,
        railHoverBg: palette.slider,
        trackBg: palette.slider,
        trackHoverBg: palette.slider,
        handleColor: palette.primary,
        handleActiveColor: palette.primary,
        // A white dot on the lavender rail needs a ring, or the stops read as holes.
        dotBorderColor: palette.indigo,
        dotActiveBorderColor: palette.indigo,
      },
      Modal: { titleFontSize: 22 },
      Calendar: navySelection,
      DatePicker: navySelection,
      Tabs: navySelection,
    },
  };
}
