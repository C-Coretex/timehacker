import type { ReactNode } from 'react';
import {
  AppstoreOutlined,
  CalendarOutlined,
  CheckSquareOutlined,
  EditOutlined,
  InfoCircleOutlined,
  SettingOutlined,
} from '@ant-design/icons';

export interface NavItem {
  to: string;
  icon: ReactNode;
  labelKey: string;
  end?: boolean;
}

/** The app's main sections, in sidebar order. */
export const PRIMARY_NAV: NavItem[] = [
  { to: '/', icon: <CalendarOutlined />, labelKey: 'nav.planning', end: true },
  { to: '/today', icon: <CheckSquareOutlined />, labelKey: 'nav.today' },
  { to: '/tasks', icon: <EditOutlined />, labelKey: 'nav.tasks' },
  { to: '/categories', icon: <AppstoreOutlined />, labelKey: 'nav.categories' },
];

/** Pages that sit with the account actions at the bottom of the navigation. */
export const SECONDARY_NAV: NavItem[] = [
  { to: '/settings', icon: <SettingOutlined />, labelKey: 'nav.settings' },
  { to: '/about', icon: <InfoCircleOutlined />, labelKey: 'nav.about' },
];
