import { useState, useEffect } from 'react';
import type { FC } from 'react';
import { Outlet, useNavigate } from 'react-router';
import {
  Badge,
  Button,
  Drawer,
  Layout as AntdLayout,
  Menu,
  Space,
  Tag,
  theme,
  Tooltip,
  Typography,
} from 'antd';
import {
  CalendarOutlined,
  FireOutlined,
  MenuOutlined,
  SnippetsOutlined,
} from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { useAuth } from 'contexts/AuthContext';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { useSettings } from 'contexts/SettingsContext';
import { fetchTasksForDay } from '../../api/tasks';
import { useIsMobile } from '../../hooks/useIsMobile';
import SidebarLogo from './SidebarLogo';
import MiniCalendar from './MiniCalendar';
import { getMainMenuItems, greetingByTime, computeSummary } from './utils';
import type { DaySummary } from './types';

const { Header, Content, Sider } = AntdLayout;

const Layout: FC = () => {
  const { isAuthenticated, user } = useAuth();
  const navigate = useNavigate();
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { selectedDate, setSelectedDate, calendarView } = useCalendarDate();
  const { weekStart } = useSettings();
  const weekStartDay = weekStart === 'monday' ? 1 : 0;

  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [summary, setSummary] = useState<DaySummary | null>(null);

  const { token: { colorBgContainer, borderRadiusLG } } = theme.useToken();

  const mainMenuItems = getMainMenuItems(isAuthenticated, t);

  useEffect(() => {
    if (!isAuthenticated) {
      setSummary(null);
      return;
    }
    fetchTasksForDay(new Date())
      .then((res) => setSummary(computeSummary(res.tasksTimeline ?? [])))
      .catch(() => setSummary(null));
  }, [isAuthenticated]);

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

  const tasksLeft = summary ? summary.fixedLeft + summary.dynamicLeft : 0;

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
          <MiniCalendar selectedDate={selectedDate} onSelect={handleMiniCalendarSelect} currentView={calendarView} weekStartDay={weekStartDay} />
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
            <MiniCalendar selectedDate={selectedDate} onSelect={handleMiniCalendarSelect} currentView={calendarView} weekStartDay={weekStartDay} />
          )}
        </Sider>
      )}

      <AntdLayout>
        <Header
          style={{
            background: colorBgContainer,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            padding: isMobile ? '0 12px' : '0 24px',
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: 8, minWidth: 0 }}>
            {isMobile && (
              <Button type="text" icon={<MenuOutlined />} onClick={() => setDrawerOpen(true)} />
            )}
            <Typography.Text
              style={{
                fontSize: isMobile ? 14 : 16,
                whiteSpace: 'nowrap',
                overflow: 'hidden',
                textOverflow: 'ellipsis',
              }}
            >
              {greetingByTime(t)}
              {user?.name ? `, ${user.name}` : ''}
              {!isMobile && ` ${t('greeting.tagline')}`}
            </Typography.Text>
          </div>

          {summary && !isMobile && (
            <Space size="middle">
              <Tooltip title={t('header.fixedRemaining', { left: summary.fixedLeft, total: summary.fixed })}>
                <Tag color="green">
                  <CalendarOutlined /> {t('header.fixedTag', { left: summary.fixedLeft, total: summary.fixed })}
                </Tag>
              </Tooltip>
              <Tooltip title={t('header.dynamicRemaining', { left: summary.dynamicLeft, total: summary.dynamic })}>
                <Tag color="orange">
                  <SnippetsOutlined /> {t('header.dynamicTag', { left: summary.dynamicLeft, total: summary.dynamic })}
                </Tag>
              </Tooltip>
              {summary.highPriority > 0 && (
                <Tooltip title={t('header.highPriority', { count: summary.highPriority })}>
                  <Badge count={summary.highPriority} overflowCount={99}>
                    <Tag color="red">
                      <FireOutlined /> {t('header.priorityLabel')}
                    </Tag>
                  </Badge>
                </Tooltip>
              )}
            </Space>
          )}

          {summary && isMobile && (
            <Tooltip
              title={
                summary.highPriority > 0
                  ? t('header.mobileTooltipWithPriority', { fixed: summary.fixedLeft, dynamic: summary.dynamicLeft, priority: summary.highPriority })
                  : t('header.mobileTooltip', { fixed: summary.fixedLeft, dynamic: summary.dynamicLeft })
              }
            >
              <Badge count={tasksLeft} overflowCount={99} size="small">
                <CalendarOutlined style={{ fontSize: 18 }} />
              </Badge>
            </Tooltip>
          )}
        </Header>

        <Content
          style={{
            margin: isMobile ? '0 8px' : '0 16px',
            marginTop: isMobile ? 8 : 16,
            display: 'flex',
            flexDirection: 'column',
            overflow: 'hidden',
          }}
        >
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
