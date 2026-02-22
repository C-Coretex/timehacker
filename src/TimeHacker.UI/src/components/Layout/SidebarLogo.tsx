import type { FC } from 'react';
import { Typography } from 'antd';
import { ClockCircleOutlined } from '@ant-design/icons';

interface Props {
  collapsed?: boolean;
  onClick: () => void;
}

const SidebarLogo: FC<Props> = ({ collapsed, onClick }) => (
  <div
    onClick={onClick}
    style={{
      height: 32,
      margin: 16,
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

export default SidebarLogo;
