import type { MenuProps } from 'antd';

export type MenuItem = Required<MenuProps>['items'][number];

export interface DaySummary {
  fixed: number;
  dynamic: number;
  fixedLeft: number;
  dynamicLeft: number;
  highPriority: number;
}
