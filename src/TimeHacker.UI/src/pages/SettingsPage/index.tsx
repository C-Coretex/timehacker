import type { FC, ReactNode } from 'react';
import { Segmented } from 'antd';
import { BgColorsOutlined, CalendarOutlined, ClockCircleOutlined, GlobalOutlined } from '@ant-design/icons';
import { useTranslation } from 'react-i18next';

import { PageHeader } from 'components/PageHeader';
import { useSettings } from 'contexts/SettingsContext';
import type { TimeFormat, WeekStart } from 'contexts/SettingsContext';
import { SettingItem } from './components/SettingItem';
import { ThemeChoice } from './components/ThemeChoice';
import './styles.css';

const SettingsSection: FC<{ title: string; children: ReactNode }> = ({ title, children }) => (
  <section className="th-settings__section">
    <h2 className="th-settings__section-title">{title}</h2>
    <div className="th-settings__rows">{children}</div>
  </section>
);

export const SettingsPage: FC = () => {
  const { timeFormat, setTimeFormat, weekStart, setWeekStart } = useSettings();
  const { t, i18n } = useTranslation();

  return (
    <div className="th-settings">
      <PageHeader title={t('settings.title')} />

      <SettingsSection title={t('settings.appearance')}>
        <SettingItem
          icon={<BgColorsOutlined />}
          label={t('settings.theme')}
          hint={t('settings.themeHint')}
          control={<ThemeChoice />}
        />
        <SettingItem
          icon={<GlobalOutlined />}
          label={t('settings.language')}
          hint={t('settings.languageHint')}
          control={
            <Segmented
              value={i18n.language?.startsWith('ru') ? 'ru' : 'en'}
              options={[
                { value: 'en', label: 'English' },
                { value: 'ru', label: 'Русский' },
              ]}
              onChange={(lng) => void i18n.changeLanguage(lng)}
            />
          }
        />
      </SettingsSection>

      <SettingsSection title={t('settings.calendar')}>
        <SettingItem
          icon={<ClockCircleOutlined />}
          label={t('settings.timeFormat')}
          hint={t('settings.timeFormatHint')}
          control={
            <Segmented<TimeFormat>
              value={timeFormat}
              options={[
                { value: '12h', label: t('settings.time12h') },
                { value: '24h', label: t('settings.time24h') },
              ]}
              onChange={setTimeFormat}
            />
          }
        />
        <SettingItem
          icon={<CalendarOutlined />}
          label={t('settings.weekStartDay')}
          hint={t('settings.weekStartDayHint')}
          control={
            <Segmented<WeekStart>
              value={weekStart}
              options={[
                { value: 'sunday', label: t('settings.sunday') },
                { value: 'monday', label: t('settings.monday') },
              ]}
              onChange={setWeekStart}
            />
          }
        />
      </SettingsSection>
    </div>
  );
};
