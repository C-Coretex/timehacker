import type { FC } from 'react';
import { Logo } from 'components/Logo';
import { useAuth } from 'contexts/AuthContext';
import { AccountNav } from '../AccountNav';
import { MiniCalendar } from '../MiniCalendar';
import { NavMenu } from '../NavMenu';
import { SidebarOrbit } from './SidebarOrbit';
import './styles.css';

/** Desktop navigation column: logo, sections, the planner's month card, then settings and account. */
export const DesktopSidebar: FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <aside className="th-sidebar">
      <SidebarOrbit />
      <div className="th-sidebar__content">
        <div className="th-sidebar__logo">
          <Logo />
        </div>
        <NavMenu />
        {isAuthenticated && <MiniCalendar />}
        <div className="th-sidebar__spacer" />
        <AccountNav />
      </div>
    </aside>
  );
};
