import type { FC, ReactNode } from 'react';
import { Space, Typography } from 'antd';

export interface SettingItemProps {
  icon: ReactNode;
  label: string;
  hint: string;
  control: ReactNode;
}

const SettingItem: FC<SettingItemProps> = ({ icon, label, hint, control }) => (
  <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
    <Space size={12} align="start">
      <div style={{ fontSize: 20, paddingTop: 2 }}>{icon}</div>
      <div>
        <Typography.Text strong>{label}</Typography.Text>
        <br />
        <Typography.Text type="secondary" style={{ fontSize: 13 }}>
          {hint}
        </Typography.Text>
      </div>
    </Space>
    {control}
  </div>
);

export default SettingItem;
