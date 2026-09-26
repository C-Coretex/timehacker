import type { FC, ReactNode } from 'react';
import './styles.css';

interface PageHeaderProps {
  title: ReactNode;
  /** Right-aligned buttons. */
  actions?: ReactNode;
  /** A row under the title — summary chips, filters. */
  children?: ReactNode;
}

/** A page's title row: heading left, actions right, optional detail row beneath. */
export const PageHeader: FC<PageHeaderProps> = ({ title, actions, children }) => (
  <header className="th-page-header">
    <div className="th-page-header__row">
      <h1 className="th-page-header__title">{title}</h1>
      {actions && <div className="th-page-header__actions">{actions}</div>}
    </div>
    {children && <div className="th-page-header__detail">{children}</div>}
  </header>
);
