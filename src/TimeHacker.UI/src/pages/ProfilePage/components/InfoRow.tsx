import type { FC, ReactNode } from 'react';

interface InfoRowProps {
  icon: ReactNode;
  label: string;
  value: string | undefined;
}

/** One read-only profile field: icon tile, small label, value (a dash when unset). */
export const InfoRow: FC<InfoRowProps> = ({ icon, label, value }) => (
  <div className="th-profile-row">
    <span className="th-profile-row__icon" aria-hidden>
      {icon}
    </span>
    <div>
      <div className="th-profile-row__label">{label}</div>
      <div className={`th-profile-row__value${value ? '' : ' is-empty'}`}>{value || '—'}</div>
    </div>
  </div>
);
