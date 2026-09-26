import type { FC } from 'react';
import { useCursorFollow } from 'hooks/useCursorFollow';

/**
 * The app's backdrop, behind the sidebar and every page: two soft blobs that trail the cursor anywhere on the
 * screen. Components paint their own surface on top of it.
 */
export const CursorGradient: FC = () => {
  const ref = useCursorFollow<HTMLDivElement>();

  return (
    <div ref={ref} className="th-cursor-gradient" aria-hidden>
      <span className="th-cursor-gradient__blob th-cursor-gradient__blob--pink" />
      <span className="th-cursor-gradient__blob th-cursor-gradient__blob--lavender" />
    </div>
  );
};
