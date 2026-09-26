import type { FC, ReactNode } from 'react';
import { NavLink } from 'react-router-dom';
import { ArrowRightOutlined } from '@ant-design/icons';

interface NavRowProps {
  icon: ReactNode;
  label: string;
  /** A route renders a link (highlighted while active); without one the row is an action button. */
  to?: string;
  /** Match the route exactly — needed for `/`, which prefixes every path. */
  end?: boolean;
  onClick?: () => void;
  /** The phone menu's trailing → arrow. */
  showArrow?: boolean;
}

export const NavRow: FC<NavRowProps> = ({ icon, label, to, end, onClick, showArrow }) => {
  const content = (
    <>
      <span className="th-nav-row__icon">{icon}</span>
      <span className="th-nav-row__label">{label}</span>
      {showArrow && <ArrowRightOutlined className="th-nav-row__arrow" />}
    </>
  );

  return to ? (
    <NavLink to={to} end={end} onClick={onClick} className={({ isActive }) => `th-nav-row${isActive ? ' is-active' : ''}`}>
      {content}
    </NavLink>
  ) : (
    <button type="button" className="th-nav-row" onClick={onClick}>
      {content}
    </button>
  );
};
