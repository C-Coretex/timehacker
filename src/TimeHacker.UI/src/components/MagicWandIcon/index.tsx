import type { CSSProperties, FC } from 'react';
import Icon from '@ant-design/icons';

const WandSvg: FC = () => (
  <svg viewBox="0 0 24 24" width="1em" height="1em" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <path d="m3 21 10-10" />
    <path d="M15 4V2M15 16v-2M8 9h2M20 9h2M17.8 11.8 19 13M17.8 6.2 19 5M12.2 6.2 11 5" />
  </svg>
);

/** The design's magic wand, used for "re-plan" — antd ships no wand glyph. */
export const MagicWandIcon: FC<{ className?: string; style?: CSSProperties }> = (props) => (
  <Icon component={WandSvg} {...props} />
);
