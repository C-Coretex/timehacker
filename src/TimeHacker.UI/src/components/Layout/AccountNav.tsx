import type { FC } from 'react';
import { useNavigate } from 'react-router-dom';
import { LoginOutlined, LogoutOutlined, UserOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { useAuth } from 'contexts/AuthContext';
import { NavRow } from './NavRow';
import { SECONDARY_NAV } from './navItems';

interface AccountNavProps {
  onNavigate?: () => void;
  showArrows?: boolean;
}

/** Settings/about and the account actions: the foot of the sidebar and of the phone menu. */
export const AccountNav: FC<AccountNavProps> = ({ onNavigate, showArrows }) => {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { isAuthenticated, logout } = useAuth();

  const handleLogout = async () => {
    onNavigate?.();
    await logout();
    navigate('/login');
  };

  return (
    <div className="th-nav th-nav--secondary">
      {SECONDARY_NAV.map((item) => (
        <NavRow
          key={item.to}
          to={item.to}
          icon={item.icon}
          label={t(item.labelKey)}
          onClick={onNavigate}
          showArrow={showArrows}
        />
      ))}
      {isAuthenticated ? (
        <>
          <NavRow to="/profile" icon={<UserOutlined />} label={t('nav.profile')} onClick={onNavigate} showArrow={showArrows} />
          <NavRow icon={<LogoutOutlined />} label={t('nav.logout')} onClick={handleLogout} />
        </>
      ) : (
        <NavRow to="/login" icon={<LoginOutlined />} label={t('nav.login')} onClick={onNavigate} showArrow={showArrows} />
      )}
    </div>
  );
};
