import { useState } from 'react';
import type { FC } from 'react';
import { Outlet, useNavigate } from 'react-router';
import { Button, Drawer, Layout as AntdLayout, Menu, theme } from 'antd';
import { MenuOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useAuth } from 'contexts/AuthContext';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import { useIsMobile } from '../../hooks/useIsMobile';
import SidebarLogo from './SidebarLogo';
import MiniCalendar from './MiniCalendar';
import { getMainMenuItems } from './utils';

const { Content, Sider } = AntdLayout;

const Layout: FC = () => {
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { selectedDate, setSelectedDate, calendarView } = useCalendarDate();
  const { weekStart } = useSettings();
  const weekStartDay = weekStart === 'monday' ? 1 : 0;

  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);

  const { token: { colorBgContainer, borderRadiusLG } } = theme.useToken();

  const mainMenuItems = getMainMenuItems(isAuthenticated, t);

  const handleLogoClick = () => {
    navigate('/');
    setDrawerOpen(false);
  };

  const handleMiniCalendarSelect = (date: Date) => {
    setSelectedDate(date);
    navigate('/');
    setDrawerOpen(false);
  };

  const menuNode = (
    <Menu
      theme="dark"
      defaultSelectedKeys={['1']}
      mode="inline"
      items={mainMenuItems}
      onClick={() => setDrawerOpen(false)}
    />
  );

  return (
    <AntdLayout style={{ height: '100vh', overflow: 'hidden' }}>
      {isMobile ? (
        <Drawer
          placement="left"
          open={drawerOpen}
          onClose={() => setDrawerOpen(false)}
          styles={{ body: { padding: 0, background: '#001529' }, wrapper: { width: 280 } }}
        >
          <SidebarLogo onClick={handleLogoClick} />
          {menuNode}
          <MiniCalendar
            selectedDate={selectedDate}
            onSelect={handleMiniCalendarSelect}
            currentView={calendarView}
            weekStartDay={weekStartDay}
          />
        </Drawer>
      ) : (
        <Sider
          collapsible
          collapsed={collapsed}
          onCollapse={(value) => setCollapsed(value)}
          width={240}
          style={{ overflow: 'auto' }}
        >
          <SidebarLogo collapsed={collapsed} onClick={handleLogoClick} />
          {menuNode}
          {!collapsed && (
            <MiniCalendar
              selectedDate={selectedDate}
              onSelect={handleMiniCalendarSelect}
              currentView={calendarView}
              weekStartDay={weekStartDay}
            />
          )}
        </Sider>
      )}

      <AntdLayout>
        <Content
          style={{
            margin: isMobile ? '8px' : '16px',
            display: 'flex',
            flexDirection: 'column',
            overflow: 'hidden',
          }}
        >
          {isMobile && (
            <Button
              type="text"
              icon={<MenuOutlined />}
              onClick={() => setDrawerOpen(true)}
              style={{ alignSelf: 'flex-start', marginBottom: 8 }}
            />
          )}
          <div
            style={{
              padding: isMobile ? 12 : 24,
              flex: 1,
              display: 'flex',
              flexDirection: 'column',
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
              overflow: 'auto',
            }}
          >
            <Outlet />
          </div>
        </Content>
      </AntdLayout>
    </AntdLayout>
  );
};

export default Layout;
