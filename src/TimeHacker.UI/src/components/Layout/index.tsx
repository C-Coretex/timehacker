import { useState, useEffect } from 'react';
import type { FC } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router';
import { Badge, Button, Drawer, Layout as AntdLayout, Menu, Space, Tag, theme, Tooltip, Typography } from 'antd';
import type { MenuProps } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  FireOutlined,
  MenuOutlined,
  ProductOutlined,
  QuestionCircleOutlined,
  SettingOutlined,
  SnippetsOutlined,
  UserOutlined,
} from '@ant-design/icons';

import { useAuth } from 'contexts/AuthContext';
import { capitalize } from 'utils/helpers';
import { fetchTasksForDay, type TaskForDayItem } from '../../api/tasks';
import { useIsMobile } from '../../hooks/useIsMobile';

const { Header, Content, Sider } = AntdLayout;

type MenuItem = Required<MenuProps>['items'][number];

const getItem = (
  key: React.Key,
  icon?: React.ReactNode,
  children?: MenuItem[]
): MenuItem => {
  return {
    key,
    icon,
    children,
    label: children ? (
      capitalize(key)
    ) : (
      <NavLink to={`/${key === 'calendar' ? '' : key}`}>
        {capitalize(key)}
      </NavLink>
    ),
  } as MenuItem;
};

const getMainMenuItems = (isAuthenticated: boolean) => [
  getItem('calendar', <CalendarOutlined />),
  getItem('tasks', <SnippetsOutlined />),
  getItem('categories', <ProductOutlined />),
  !isAuthenticated
    ? getItem('login', <UserOutlined />)
    : getItem('profile', <UserOutlined />),
  getItem('help', <QuestionCircleOutlined />),
  getItem('about', <QuestionCircleOutlined />),
  getItem('settings', <SettingOutlined />),
];

const greetingByTime = (): string => {
  const hour = new Date().getHours();
  if (hour < 12) return 'Good morning';
  if (hour < 18) return 'Good afternoon';
  return 'Good evening';
};

interface DaySummary {
  fixed: number;
  dynamic: number;
  fixedLeft: number;
  dynamicLeft: number;
  highPriority: number;
}

const computeSummary = (tasks: TaskForDayItem[]): DaySummary => {
  const now = new Date();
  const nowMinutes = now.getHours() * 60 + now.getMinutes();

  let fixed = 0;
  let dynamic = 0;
  let fixedLeft = 0;
  let dynamicLeft = 0;
  let highPriority = 0;

  for (const t of tasks) {
    const endParts = t.timeRange.end.split(/[.:]/).map(Number);
    const endMinutes =
      endParts.length >= 4
        ? (endParts[0] ?? 0) * 1440 + (endParts[1] ?? 0) * 60 + (endParts[2] ?? 0)
        : (endParts[0] ?? 0) * 60 + (endParts[1] ?? 0);

    if (t.isFixed) {
      fixed++;
      if (endMinutes > nowMinutes) fixedLeft++;
    } else {
      dynamic++;
      if (endMinutes > nowMinutes) dynamicLeft++;
    }
    if (t.task.priority >= 8) highPriority++;
  }

  return { fixed, dynamic, fixedLeft, dynamicLeft, highPriority };
};

const SidebarLogo: FC<{ collapsed?: boolean; onClick: () => void }> = ({ collapsed, onClick }) => (
  <div
    onClick={onClick}
    style={{
      height: '32px',
      margin: '16px',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      gap: 8,
      cursor: 'pointer',
    }}
  >
    <ClockCircleOutlined style={{ color: '#1890ff', fontSize: collapsed ? 20 : 18 }} />
    {!collapsed && (
      <Typography.Text strong style={{ color: '#fff', fontSize: 16, whiteSpace: 'nowrap' }}>
        TimeHacker
      </Typography.Text>
    )}
  </div>
);

const Layout: FC = () => {
  const { isAuthenticated, user } = useAuth();
  const navigate = useNavigate();
  const { isMobile } = useIsMobile();

  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [summary, setSummary] = useState<DaySummary | null>(null);

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const mainMenuItems = getMainMenuItems(isAuthenticated);

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
          width={250}
          styles={{ body: { padding: 0, background: '#001529' } }}
        >
          <SidebarLogo onClick={handleLogoClick} />
          {menuNode}
        </Drawer>
      ) : (
        <Sider
          collapsible
          collapsed={collapsed}
          onCollapse={(value) => setCollapsed(value)}
        >
          <SidebarLogo collapsed={collapsed} onClick={handleLogoClick} />
          {menuNode}
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
              <Button
                type="text"
                icon={<MenuOutlined />}
                onClick={() => setDrawerOpen(true)}
              />
            )}
            <Typography.Text
              style={{ fontSize: isMobile ? 14 : 16, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}
            >
              {greetingByTime()}
              {user?.name ? `, ${user.name}` : ''}
              {!isMobile && ' — make every minute count'}
            </Typography.Text>
          </div>

          {summary && !isMobile && (
            <Space size="middle">
              <Tooltip title={`${summary.fixedLeft} of ${summary.fixed} fixed tasks remaining`}>
                <Tag color="green">
                  <CalendarOutlined /> Fixed: {summary.fixedLeft}/{summary.fixed}
                </Tag>
              </Tooltip>
              <Tooltip title={`${summary.dynamicLeft} of ${summary.dynamic} dynamic tasks remaining`}>
                <Tag color="orange">
                  <SnippetsOutlined /> Dynamic: {summary.dynamicLeft}/{summary.dynamic}
                </Tag>
              </Tooltip>
              {summary.highPriority > 0 && (
                <Tooltip title={`${summary.highPriority} high priority task${summary.highPriority > 1 ? 's' : ''} today`}>
                  <Badge count={summary.highPriority} overflowCount={99}>
                    <Tag color="red">
                      <FireOutlined /> Priority
                    </Tag>
                  </Badge>
                </Tooltip>
              )}
            </Space>
          )}
          {summary && isMobile && (
            <Tooltip title={`${summary.fixedLeft}F + ${summary.dynamicLeft}D left${summary.highPriority > 0 ? `, ${summary.highPriority} high priority` : ''}`}>
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
