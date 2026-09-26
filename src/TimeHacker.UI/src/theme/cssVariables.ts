import { darkPalette, fonts, lightPalette, radii } from './palette';
import type { Palette } from './palette';

// primaryHover → primary-hover, lavender100 → lavender-100 (digits start a segment too).
const toKebab = (key: string) =>
  key.replace(/[A-Z]/g, (letter) => `-${letter.toLowerCase()}`).replace(/\d+/g, (digits) => `-${digits}`);

const declarations = (palette: Palette) =>
  Object.entries(palette)
    .map(([key, value]) => `--th-${toKebab(key)}: ${value};`)
    .join('');

const sharedDeclarations = [
  `--th-radius-control: ${radii.control}px;`,
  `--th-radius-card: ${radii.card}px;`,
  `--th-radius-pill: ${radii.pill}px;`,
  `--th-font-sans: ${fonts.sans};`,
  `--th-font-digital: ${fonts.digital};`,
].join('');

/**
 * Publishes the palette as `--th-*` custom properties on `:root`, with the dark set under `:root.dark`
 * (ThemeContext toggles that class). antd scopes its own variables per component, so plain CSS — the
 * calendar, the shell — reads these instead. Runs once, before the first render.
 */
export function installCssVariables(): void {
  const style = document.createElement('style');
  style.dataset.theme = 'timehacker';
  style.textContent =
    `:root{${sharedDeclarations}${declarations(lightPalette)}color-scheme:light;}` +
    `:root.dark{${declarations(darkPalette)}color-scheme:dark;}`;
  document.head.appendChild(style);
}
