import { Card, Switch, Typography } from 'antd';
import { BulbOutlined } from '@ant-design/icons';
import type { FC } from 'react';
import { useTheme } from 'contexts/ThemeContext';

const { Title, Text } = Typography;

const SettingsPage: FC = () => {
  const { darkMode, updateDarkMode } = useTheme();

  return (
    <div style={{ maxWidth: 600, padding: '0 16px' }}>
      <Title level={3}>Settings</Title>

      <Card title="Appearance" style={{ marginTop: 16 }}>
        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <BulbOutlined style={{ fontSize: 20 }} />
            <div>
              <Text strong>Dark mode</Text>
              <br />
              <Text type="secondary">
                Switch between light and dark themes
              </Text>
            </div>
          </div>
          <Switch
            checked={darkMode}
            onChange={(checked) => updateDarkMode(checked)}
          />
        </div>
      </Card>
    </div>
  );
};

export default SettingsPage;
