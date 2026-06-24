import type { FC, ReactNode } from 'react';
import { Typography } from 'antd';

interface InfoRowProps {
  icon: ReactNode;
  label: string;
  value: string | undefined;
}

export const InfoRow: FC<InfoRowProps> = ({ icon, label, value }) => (
  <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px 0' }}>
    <span style={{ fontSize: 18, opacity: 0.6 }}>{icon}</span>
    <div>
      <Typography.Text type="secondary" style={{ fontSize: 12 }}>
        {label}
      </Typography.Text>
      <br />
      <Typography.Text strong>{value || '—'}</Typography.Text>
    </div>
  </div>
);

