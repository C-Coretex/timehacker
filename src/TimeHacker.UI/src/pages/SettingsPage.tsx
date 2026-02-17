import { Card, Select, Switch, Space, Typography } from 'antd';
import { BulbOutlined, ClockCircleOutlined, CalendarOutlined, GlobalOutlined } from '@ant-design/icons';
import type { FC, ReactNode } from 'react';
import { useMemo } from 'react';
import { useTheme } from 'contexts/ThemeContext';
import { useSettings } from 'contexts/SettingsContext';
import type { TimeFormat, WeekStart } from 'contexts/SettingsContext';
import { useTranslation } from 'react-i18next';

const { Title, Text } = Typography;

interface SettingItemProps {
  icon: ReactNode;
  label: string;
  hint: string;
  control: ReactNode;
}

const SettingItem: FC<SettingItemProps> = ({ icon, label, hint, control }) => (
  <div
    style={{
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'space-between',
    }}
  >
    <Space size={12} align="start">
      <div style={{ fontSize: 20, paddingTop: 2 }}>{icon}</div>
      <div>
        <Text strong>{label}</Text>
        <br />
        <Text type="secondary" style={{ fontSize: 13 }}>
          {hint}
        </Text>
      </div>
    </Space>
    {control}
  </div>
);

const SettingsPage: FC = () => {
  const { darkMode, updateDarkMode } = useTheme();
  const { timeFormat, setTimeFormat, weekStart, setWeekStart } = useSettings();
  const { t, i18n } = useTranslation();

  const languageOptions = useMemo(
    () => [
      { value: 'en', label: 'English' },
      { value: 'ru', label: 'Русский' },
    ],
    []
  );

  const timeFormatOptions = useMemo(
    () => [
      { value: '12h' as TimeFormat, label: t('settings.time12h') },
      { value: '24h' as TimeFormat, label: t('settings.time24h') },
    ],
    [t]
  );

  const weekStartOptions = useMemo(
    () => [
      { value: 'sunday' as WeekStart, label: t('settings.sunday') },
      { value: 'monday' as WeekStart, label: t('settings.monday') },
    ],
    [t]
  );

  return (
    <div style={{ maxWidth: 600, padding: '0 16px' }}>
      <Title level={3}>{t('settings.title')}</Title>

      <Card title={t('settings.appearance')} style={{ marginTop: 16 }}>
        <Space direction="vertical" size={24} style={{ width: '100%' }}>
          <SettingItem
            icon={<BulbOutlined />}
            label={t('settings.darkMode')}
            hint={t('settings.darkModeHint')}
            control={<Switch checked={darkMode} onChange={updateDarkMode} />}
          />
          <SettingItem
            icon={<GlobalOutlined />}
            label={t('settings.language')}
            hint={t('settings.languageHint')}
            control={
              <Select
                value={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
                options={languageOptions}
                onChange={(lng) => i18n.changeLanguage(lng)}
                style={{ width: 140 }}
              />
            }
          />
        </Space>
      </Card>

      <Card title={t('settings.calendar')} style={{ marginTop: 16 }}>
        <Space direction="vertical" size={24} style={{ width: '100%' }}>
          <SettingItem
            icon={<ClockCircleOutlined />}
            label={t('settings.timeFormat')}
            hint={t('settings.timeFormatHint')}
            control={
              <Select
                value={timeFormat}
                options={timeFormatOptions}
                onChange={setTimeFormat}
                style={{ width: 140 }}
              />
            }
          />
          <SettingItem
            icon={<CalendarOutlined />}
            label={t('settings.weekStartDay')}
            hint={t('settings.weekStartDayHint')}
            control={
              <Select
                value={weekStart}
                options={weekStartOptions}
                onChange={setWeekStart}
                style={{ width: 140 }}
              />
            }
          />
        </Space>
      </Card>
    </div>
  );
};

export default SettingsPage;
