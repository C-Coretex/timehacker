import type { FC } from 'react';

const ORBIT_DOTS = [
  [38, 22],
  [12, 92],
  [52, 168],
  [118, 196],
];

/** The sidebar's faint dotted orbit, clipped to the sidebar's edge. */
export const SidebarOrbit: FC = () => (
  <svg className="th-sidebar__orbit" viewBox="0 0 220 220" aria-hidden>
    <circle cx="140" cy="100" r="128" />
    {ORBIT_DOTS.map(([cx, cy]) => (
      <circle key={`${cx}-${cy}`} className="th-sidebar__orbit-dot" cx={cx} cy={cy} r="2.2" />
    ))}
  </svg>
);
