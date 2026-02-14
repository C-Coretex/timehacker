import { useState } from 'react';
import type { FC } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router';
import { Layout as AntdLayout, Menu, theme, Typography } from 'antd';
import type { MenuProps } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  ProductOutlined,
  QuestionCircleOutlined,
  SettingOutlined,
  SnippetsOutlined,
  UserOutlined,
} from '@ant-design/icons';

import { useAuth } from 'contexts/AuthContext';
import { capitalize } from 'utils/helpers';

const { Header, Content, Footer, Sider } = AntdLayout;

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

const Layout: FC = () => {
  const { isAuthenticated, user } = useAuth();
  const navigate = useNavigate();

  const [collapsed, setCollapsed] = useState(false);

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const mainMenuItems = getMainMenuItems(isAuthenticated);

  return (
    <AntdLayout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={(value) => setCollapsed(value)}
      >
        <div
          onClick={() => navigate('/')}
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
        <Menu
          theme="dark"
          defaultSelectedKeys={['1']}
          mode="inline"
          items={mainMenuItems}
        />
      </Sider>
      <AntdLayout>
        <Header
          style={{
            background: colorBgContainer,
            display: 'flex',
            alignItems: 'center',
            padding: '0 24px',
          }}
        >
          <Typography.Text style={{ fontSize: 16 }}>
            {greetingByTime()}
            {user?.name ? `, ${user.name}` : ''}
            {' — make every minute count'}
          </Typography.Text>
        </Header>
        <Content style={{ margin: '0 16px', marginTop: 16 }}>
          <div
            style={{
              padding: 24,
              minHeight: 360,
              background: colorBgContainer,
              borderRadius: borderRadiusLG,
            }}
          >
            <Outlet />
          </div>
        </Content>
        <Footer style={{ textAlign: 'center' }}>
          TimeHacker
        </Footer>
      </AntdLayout>
    </AntdLayout>
  );
};

export default Layout;
