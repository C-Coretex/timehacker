import type { FC } from 'react';
import { useTranslation } from 'react-i18next';
import { NavRow } from './NavRow';
import { PRIMARY_NAV } from './navItems';

interface NavMenuProps {
  onNavigate?: () => void;
  showArrows?: boolean;
}

/** The main sections as nav rows — shared by the desktop sidebar and the phone menu. */
export const NavMenu: FC<NavMenuProps> = ({ onNavigate, showArrows }) => {
  const { t } = useTranslation();

  return (
    <nav className="th-nav" aria-label={t('shell.mainNavigation')}>
      {PRIMARY_NAV.map((item) => (
        <NavRow
          key={item.to}
          to={item.to}
          end={item.end}
          icon={item.icon}
          label={t(item.labelKey)}
          onClick={onNavigate}
          showArrow={showArrows}
        />
      ))}
    </nav>
  );
};
