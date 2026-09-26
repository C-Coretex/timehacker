import type { FC } from 'react';

/** A from – to pair for a ListCard's `aside`, stacked so it stays narrow enough to leave the title room on a phone. */
export const ListCardRange: FC<{ from: string; to: string }> = ({ from, to }) => (
  <span className="th-list-card-range">
    <span>{from}</span>
    <span className="th-list-card-range__to">– {to}</span>
  </span>
);
