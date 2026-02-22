import { NavLink } from 'react-router';
import {
  CalendarOutlined,
  ProductOutlined,
  QuestionCircleOutlined,
  SettingOutlined,
  SnippetsOutlined,
  UserOutlined,
} from '@ant-design/icons';
import type { TFunction } from 'i18next';
import type { MenuItem, DaySummary } from './types';
import type { TaskForDayItem } from '../../api/tasks';

export const getItem = (
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
      <NavLink to={`/${key === 'calendar' ? '' : key}`}>{label}</NavLink>
    ),
  } as MenuItem;
};

export const getMainMenuItems = (isAuthenticated: boolean, t: TFunction): MenuItem[] => [
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

export const greetingByTime = (t: TFunction): string => {
  const hour = new Date().getHours();
  if (hour < 12) return t('greeting.morning');
  if (hour < 18) return t('greeting.afternoon');
  return t('greeting.evening');
};

export const computeSummary = (tasks: TaskForDayItem[]): DaySummary => {
  const now = new Date();
  const nowMinutes = now.getHours() * 60 + now.getMinutes();

  let fixed = 0;
  let dynamic = 0;
  let fixedLeft = 0;
  let dynamicLeft = 0;
  let highPriority = 0;

  for (const task of tasks) {
    const endParts = task.timeRange.end.split(/[.:]/).map(Number);
    const endMinutes =
      endParts.length >= 4
        ? (endParts[0] ?? 0) * 1440 + (endParts[1] ?? 0) * 60 + (endParts[2] ?? 0)
        : (endParts[0] ?? 0) * 60 + (endParts[1] ?? 0);

    if (task.isFixed) {
      fixed++;
      if (endMinutes > nowMinutes) fixedLeft++;
    } else {
      dynamic++;
      if (endMinutes > nowMinutes) dynamicLeft++;
    }
    if (task.task.priority >= 8) highPriority++;
  }

  return { fixed, dynamic, fixedLeft, dynamicLeft, highPriority };
};
