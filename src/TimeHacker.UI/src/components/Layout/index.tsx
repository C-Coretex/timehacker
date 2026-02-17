import { useState, useEffect } from 'react';
import type { FC } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router';
import { Badge, Button, Calendar, Drawer, Layout as AntdLayout, Menu, Space, Tag, theme, Tooltip, Typography } from 'antd';
import type { MenuProps } from 'antd';
import {
  CalendarOutlined,
  ClockCircleOutlined,
  DoubleLeftOutlined,
  DoubleRightOutlined,
  FireOutlined,
  LeftOutlined,
  MenuOutlined,
  ProductOutlined,
  QuestionCircleOutlined,
  RightOutlined,
  SettingOutlined,
  SnippetsOutlined,
  UserOutlined,
} from '@ant-design/icons';
import type { TFunction } from 'i18next';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';

import { useAuth } from 'contexts/AuthContext';
import { useCalendarDate } from 'contexts/CalendarDateContext';
import { fetchTasksForDay, type TaskForDayItem } from '../../api/tasks';
import { useIsMobile } from '../../hooks/useIsMobile';

const { Header, Content, Sider } = AntdLayout;

type MenuItem = Required<MenuProps>['items'][number];

const getItem = (
  key: React.Key,
  icon?: React.ReactNode,
  t?: TFunction,
  children?: MenuItem[]
): MenuItem => {
  const label = t ? t(`nav.${key}`) : String(key);
  return {
    key,
    icon,
    children,
    label: children ? (
      label
    ) : (
      <NavLink to={`/${key === 'calendar' ? '' : key}`}>
        {label}
      </NavLink>
    ),
  } as MenuItem;
};

const getMainMenuItems = (isAuthenticated: boolean, t: TFunction) => [
  getItem('calendar', <CalendarOutlined />, t),
  getItem('tasks', <SnippetsOutlined />, t),
  getItem('categories', <ProductOutlined />, t),
  !isAuthenticated
    ? getItem('login', <UserOutlined />, t)
    : getItem('profile', <UserOutlined />, t),
  getItem('help', <QuestionCircleOutlined />, t),
  getItem('about', <QuestionCircleOutlined />, t),
  getItem('settings', <SettingOutlined />, t),
];

const greetingByTime = (t: TFunction): string => {
  const hour = new Date().getHours();
  if (hour < 12) return t('greeting.morning');
  if (hour < 18) return t('greeting.afternoon');
  return t('greeting.evening');
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

const MiniCalendar: FC<{ selectedDate: Date; onSelect: (date: Date) => void }> = ({ selectedDate, onSelect }) => (
  <div className="mini-cal" style={{ padding: '4px 8px', borderTop: '1px solid rgba(255,255,255,0.1)' }}>
    <Calendar
      fullscreen={false}
      value={dayjs(selectedDate)}
      onSelect={(date) => onSelect(date.toDate())}
      headerRender={({ value, onChange }) => (
        <div style={{ padding: '4px 0' }}>
          <div style={{ textAlign: 'center', fontWeight: 600, fontSize: 13, marginBottom: 4 }}>
            {value.format('MMMM YYYY')}
          </div>
          <div style={{ display: 'flex', justifyContent: 'center', gap: 4 }}>
            <Button
              type="text"
              size="small"
              icon={<DoubleLeftOutlined />}
              onClick={() => onChange(value.subtract(1, 'year'))}
              aria-label="Previous year"
            />
            <Button
              type="text"
              size="small"
              icon={<LeftOutlined />}
              onClick={() => onChange(value.subtract(1, 'month'))}
              aria-label="Previous month"
            />
            <Button
              type="text"
              size="small"
              onClick={() => onChange(dayjs())}
              aria-label="Today"
            >
              <CalendarOutlined />
            </Button>
            <Button
              type="text"
              size="small"
              icon={<RightOutlined />}
              onClick={() => onChange(value.add(1, 'month'))}
              aria-label="Next month"
            />
            <Button
              type="text"
              size="small"
              icon={<DoubleRightOutlined />}
              onClick={() => onChange(value.add(1, 'year'))}
              aria-label="Next year"
            />
          </div>
        </div>
      )}
    />
  </div>
);

const Layout: FC = () => {
  const { isAuthenticated, user } = useAuth();
  const navigate = useNavigate();
  const { isMobile } = useIsMobile();
  const { t } = useTranslation();
  const { selectedDate, setSelectedDate } = useCalendarDate();

  const [collapsed, setCollapsed] = useState(false);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [summary, setSummary] = useState<DaySummary | null>(null);

  const {
    token: { colorBgContainer, borderRadiusLG },
  } = theme.useToken();

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
          <MiniCalendar selectedDate={selectedDate} onSelect={handleMiniCalendarSelect} />
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
            <MiniCalendar selectedDate={selectedDate} onSelect={handleMiniCalendarSelect} />
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
              <Button
                type="text"
                icon={<MenuOutlined />}
                onClick={() => setDrawerOpen(true)}
              />
            )}
            <Typography.Text
              style={{ fontSize: isMobile ? 14 : 16, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}
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
            <Tooltip title={
              summary.highPriority > 0
                ? t('header.mobileTooltipWithPriority', { fixed: summary.fixedLeft, dynamic: summary.dynamicLeft, priority: summary.highPriority })
                : t('header.mobileTooltip', { fixed: summary.fixedLeft, dynamic: summary.dynamicLeft })
            }>
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
