import type { FC } from 'react';
import { Outlet } from 'react-router-dom';
import { CursorGradient } from './CursorGradient';
import { DesktopSidebar } from './DesktopSidebar';
import { MobileNav } from './MobileNav';
import './styles.css';

/**
 * App shell: the cursor glow across the whole backdrop, the sidebar on desktop or top bar + menu on phones,
 * and the page. Nothing in the shell paints a surface — only components do, so the glow shows everywhere else.
 */
export const Layout: FC = () => (
  <div className="th-shell">
    <CursorGradient />
    <DesktopSidebar />
    <MobileNav />
    <main className="th-shell__main">
      <div className="th-shell__panel">
        <Outlet />
      </div>
    </main>
  </div>
);
