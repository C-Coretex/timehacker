import { useMemo } from 'react';
import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import { App as AntdApp, ConfigProvider } from 'antd';
import enUS from 'antd/locale/en_US';
import ruRU from 'antd/locale/ru_RU';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import 'i18n/index';
import { AuthProvider } from 'contexts/AuthContext';
import { ThemeProvider, useTheme } from 'contexts/ThemeContext';
import { CalendarDateProvider } from 'contexts/CalendarDateContext';
import { SettingsProvider } from 'contexts/SettingsContext';
import { AppRoutes } from 'config/AppRoutes';
import { buildAntdTheme } from 'theme/antdTheme';
import { ErrorBoundary } from 'components/ErrorBoundary';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

// Created once at module scope so re-renders of <App> don't discard the cache.
const queryClient = new QueryClient();

const AppRoutesWrapper = () => {
  const element = useRoutes(AppRoutes);
  return element;
};

const ThemedApp = ({ children }: { children: React.ReactNode }) => {
  const { darkMode } = useTheme();
  const { i18n } = useTranslation();
  // A new theme object re-renders every antd consumer in the tree, so keep it stable per mode.
  const antdTheme = useMemo(() => buildAntdTheme(darkMode), [darkMode]);

  const antdLocale = i18n.language?.startsWith('ru') ? ruRU : enUS;
  dayjs.locale(i18n.language?.startsWith('ru') ? 'ru' : 'en');

  return (
    <ConfigProvider theme={antdTheme} locale={antdLocale}>
      <AntdApp>
        {children}
      </AntdApp>
    </ConfigProvider>
  );
};

export const App = () => {
  return (
    <Router basename="/app">
      <ThemeProvider>
        <ThemedApp>
          <SettingsProvider>
            <AuthProvider>
              <CalendarDateProvider>
                <QueryClientProvider client={queryClient}>
                  <ErrorBoundary>
                    <AppRoutesWrapper />
                  </ErrorBoundary>
                </QueryClientProvider>
              </CalendarDateProvider>
            </AuthProvider>
          </SettingsProvider>
        </ThemedApp>
      </ThemeProvider>
    </Router>
  );
};

