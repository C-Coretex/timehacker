import { Card, Select, Switch, Typography } from 'antd';
import { BulbOutlined, GlobalOutlined } from '@ant-design/icons';
import type { FC } from 'react';
import { useTheme } from 'contexts/ThemeContext';
import { useTranslation } from 'react-i18next';

const { Title, Text } = Typography;

const LANGUAGE_OPTIONS = [
  { value: 'en', label: 'English' },
  { value: 'ru', label: 'Русский' },
];

const SettingsPage: FC = () => {
  const { darkMode, updateDarkMode } = useTheme();
  const { t, i18n } = useTranslation();

  return (
    <div style={{ maxWidth: 600, padding: '0 16px' }}>
      <Title level={3}>{t('settings.title')}</Title>

      <Card title={t('settings.appearance')} style={{ marginTop: 16 }}>
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
              <Text strong>{t('settings.darkMode')}</Text>
              <br />
              <Text type="secondary">
                {t('settings.darkModeHint')}
              </Text>
            </div>
          </div>
          <Switch
            checked={darkMode}
            onChange={(checked) => updateDarkMode(checked)}
          />
        </div>

        <div
          style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            marginTop: 24,
          }}
        >
          <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            <GlobalOutlined style={{ fontSize: 20 }} />
            <div>
              <Text strong>{t('settings.language')}</Text>
              <br />
              <Text type="secondary">
                {t('settings.languageHint')}
              </Text>
            </div>
          </div>
          <Select
            value={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
            options={LANGUAGE_OPTIONS}
            onChange={(lng) => i18n.changeLanguage(lng)}
            style={{ width: 120 }}
          />
        </div>
      </Card>
    </div>
  );
};

export default SettingsPage;
