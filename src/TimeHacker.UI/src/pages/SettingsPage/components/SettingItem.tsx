import type { FC, ReactNode } from 'react';

export interface SettingItemProps {
  icon: ReactNode;
  label: string;
  hint: string;
  control: ReactNode;
}

/** One settings row: icon tile, label with its hint, and the control (below the text on phones). */
export const SettingItem: FC<SettingItemProps> = ({ icon, label, hint, control }) => (
  <div className="th-setting">
    <span className="th-setting__icon" aria-hidden>
      {icon}
    </span>
    <div className="th-setting__text">
      <div className="th-setting__label">{label}</div>
      <div className="th-setting__hint">{hint}</div>
    </div>
    <div className="th-setting__control">{control}</div>
  </div>
);
