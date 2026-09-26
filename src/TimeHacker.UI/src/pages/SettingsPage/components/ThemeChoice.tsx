import type { CSSProperties, FC, ReactNode } from 'react';
import { MoonOutlined, SunOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { useTheme } from 'contexts/ThemeContext';
import { darkPalette, lightPalette } from 'theme/palette';
import type { Palette } from 'theme/palette';

// Each tile previews its own palette, whichever mode is active — so it cannot read the live --th-* set.
const previewColours = (palette: Palette) =>
  ({
    '--th-tile-glow-pink': palette.gradientPink,
    '--th-tile-glow-lavender': palette.gradientLavender,
    '--th-tile-nav': palette.navActiveBg,
    '--th-tile-panel': palette.panel,
    '--th-tile-page': palette.page,
    '--th-tile-line': palette.grid,
    '--th-tile-accent': palette.primary,
  }) as CSSProperties;

interface ThemeTileProps {
  palette: Palette;
  icon: ReactNode;
  label: string;
  selected: boolean;
  onSelect: () => void;
}

const ThemeTile: FC<ThemeTileProps> = ({ palette, icon, label, selected, onSelect }) => (
  <button
    type="button"
    role="radio"
    aria-checked={selected}
    className={`th-theme-tile${selected ? ' is-selected' : ''}`}
    style={previewColours(palette)}
    onClick={onSelect}
  >
    <span className="th-theme-tile__preview" aria-hidden>
      <span className="th-theme-tile__sidebar">
        <span className="th-theme-tile__nav" />
      </span>
      <span className="th-theme-tile__panel">
        <span className="th-theme-tile__line" />
        <span className="th-theme-tile__line th-theme-tile__line--short" />
        <span className="th-theme-tile__dot" />
      </span>
    </span>
    <span className="th-theme-tile__label">
      {icon}
      {label}
    </span>
  </button>
);

/** Light / dark as two preview tiles — the app's only theme switch. */
export const ThemeChoice: FC = () => {
  const { t } = useTranslation();
  const { darkMode, updateDarkMode } = useTheme();

  return (
    <div className="th-theme-choice" role="radiogroup" aria-label={t('settings.theme')}>
      <ThemeTile
        palette={lightPalette}
        icon={<SunOutlined />}
        label={t('settings.light')}
        selected={!darkMode}
        onSelect={() => updateDarkMode(false)}
      />
      <ThemeTile
        palette={darkPalette}
        icon={<MoonOutlined />}
        label={t('settings.dark')}
        selected={darkMode}
        onSelect={() => updateDarkMode(true)}
      />
    </div>
  );
};
