import React, { useState } from 'react';
import type { FC } from 'react';
import { NavLink, Outlet, useLocation } from 'react-router';
import { Breadcrumb, Layout as AntdLayout, Menu, theme } from 'antd';
import type { MenuProps } from 'antd';
import {
  CalendarOutlined,
  ProductOutlined,
  QuestionCircleOutlined,
  SettingOutlined,
  SnippetsOutlined,
  UserOutlined,
} from '@ant-design/icons';

import { capitalize } from 'utils/helpers';

const { Header, Content, Footer, Sider } = AntdLayout;

type MenuItem = Required<MenuProps>['items'][number];

// const findActiveLink = (url: string, menuItems: MenuItem[]) => {
//     for (const menuItem of menuItems) {
//       if (url.includes('profile')) {
//         return 'profile'
//       } else if (url === menuItem?.label?.props?.href) {
//         return String(menuItem.key)
//       }
//     }

//     return null;
// }

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

const Layout: FC = () => {
  const { isAuthenticated, logout } = useAuth();
  const location = useLocation();

  const [collapsed, setCollapsed] = useState(false);

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

  const mainMenuItems = getMainMenuItems(isAuthenticated);
  // const activeLink = findActiveLink(location.pathname, mainMenuItems);

  const breadcrumbItems = [
    {
      title: 'User',
    },
    {
      title: 'Bill',
    },
  ];

  <Breadcrumb style={{ margin: '16px 0' }} items={breadcrumbItems} />;

  return (
    <AntdLayout style={{ minHeight: '100vh' }}>
      <Sider
        collapsible
        collapsed={collapsed}
        onCollapse={(value) => setCollapsed(value)}
      >
        <div
          className="demo-logo-vertical"
          style={{
            height: '32px',
            margin: '16px',
            background: 'rgba(255, 255, 255, .2)',
            borderRadius: '6px',
          }}
        />
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
          }}
        >
          <i>~TimeHacker~</i>
        </Header>
        <Content style={{ margin: '0 16px' }}>
          <Breadcrumb
            style={{ margin: '16px 0' }}
            items={breadcrumbItems}
            separator=">"
          />

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
          TimeHacker &copy;2024 &mdash; {new Date().getFullYear()} All rights
          reserved.
        </Footer>
      </AntdLayout>
    </AntdLayout>
  );
};

export default Layout;
