import React, { useState } from 'react';

import { NavLink, Outlet, useLocation } from 'react-router';
import type { Route } from './+types/layout';

import {
    CalendarOutlined,
    ProductOutlined,
    QuestionCircleOutlined,
    SettingOutlined,
    SnippetsOutlined,
    UserOutlined,
} from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { Breadcrumb, Layout as AntdLayout, Menu, theme } from 'antd';

import { capitalize } from '~/utils/helpers';

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
    children?: MenuItem[],
): MenuItem => {
    return {
      key,
      icon,
      children,
      label: children ? capitalize(key) : <NavLink to={`/${key === 'calendar' ? '' : key}`}>{capitalize(key)}</NavLink>
    } as MenuItem;
}

const getMainMenuItems = (isAuthenticated: boolean) => ([
    getItem('calendar', <CalendarOutlined />),
    getItem('tasks', <SnippetsOutlined />),
    getItem('categories', <ProductOutlined />),
    ...(!isAuthenticated ? [
        getItem('user', <UserOutlined />, [
        getItem('profile'),
        getItem('logout'),
        ]),
    ] : [
        getItem('login', <ProductOutlined />),
    ]),
    getItem('help', <QuestionCircleOutlined />),
    getItem('settings', <SettingOutlined />),
]);

export function meta({}: Route.MetaArgs) {
    // !!!TODO
    return [
        { title: 'TimeHacker' },
        { name: 'description', content: 'Welcome to TimeHacker!' },
    ];
}

export const loader = async ({ request }: { request: Request }) => {
    // !!!TODO
    return { aa: request.url };
};

export default function Layout({ loaderData }: Route.ComponentProps) {
    const location = useLocation();

    const [collapsed, setCollapsed] = useState(false);

    const {
        token: { colorBgContainer, borderRadiusLG },
    } = theme.useToken();

    const mainMenuItems = getMainMenuItems(false);
    // const activeLink = findActiveLink(location.pathname, mainMenuItems);


    return (
        <AntdLayout style={{ minHeight: '100vh' }}>
            <Sider collapsible collapsed={collapsed} onCollapse={(value) => setCollapsed(value)}>
                <div 
                    className="demo-logo-vertical" 
                    style={{
                        height: '32px',
                        margin: '16px',
                        background: 'rgba(255, 255, 255, .2)',
                        borderRadius: '6px',
                    }}
                />
                <Menu theme="dark" defaultSelectedKeys={['1']} mode="inline" items={mainMenuItems} />
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
                    <Breadcrumb style={{ margin: '16px 0' }}>
                    <Breadcrumb.Item>User</Breadcrumb.Item>
                    <Breadcrumb.Item>Bill</Breadcrumb.Item>
                    </Breadcrumb>
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
                    TimeHacker ©{new Date().getFullYear()}
                </Footer>
            </AntdLayout>
        </AntdLayout>
    );
};