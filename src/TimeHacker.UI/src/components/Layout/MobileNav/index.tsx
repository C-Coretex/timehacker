import { useState } from 'react';
import type { FC } from 'react';
import { Button, Drawer } from 'antd';
import { CloseOutlined, MenuOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';
import { Logo } from 'components/Logo';
import { AccountNav } from '../AccountNav';
import { NavMenu } from '../NavMenu';
import './styles.css';

/** Phone chrome: a sticky logo bar whose hamburger opens the whole navigation as a full-screen menu. */
export const MobileNav: FC = () => {
  const { t } = useTranslation();
  const [open, setOpen] = useState(false);
  const close = () => setOpen(false);

  return (
    <header className="th-mobile-topbar">
      <Logo onClick={close} />
      <Button type="text" icon={<MenuOutlined />} aria-label={t('shell.openMenu')} onClick={() => setOpen(true)} />

      <Drawer
        open={open}
        onClose={close}
        placement="right"
        size="100%"
        closable={false}
        rootClassName="th-mobile-menu"
        title={
          <div className="th-mobile-menu__header">
            <Logo onClick={close} />
            <Button type="text" icon={<CloseOutlined />} aria-label={t('shell.closeMenu')} onClick={close} />
          </div>
        }
      >
        <NavMenu onNavigate={close} showArrows />
        <AccountNav onNavigate={close} showArrows />
      </Drawer>
    </header>
  );
};
