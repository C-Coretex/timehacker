import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import { ConfigProvider, theme } from 'antd';
import enUS from 'antd/locale/en_US';
import ruRU from 'antd/locale/ru_RU';
import { useTranslation } from 'react-i18next';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import 'i18n/index';
import { AuthProvider } from 'contexts/AuthContext';
import { ThemeProvider, useTheme } from 'contexts/ThemeContext';
import AppRoutes from 'config/AppRoutes';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const AppRoutesWrapper = () => {
  const element = useRoutes(AppRoutes);
  return element;
};

const ThemedApp = ({ children }: { children: React.ReactNode }) => {
  const { darkMode } = useTheme();
  const { i18n } = useTranslation();

  const antdLocale = i18n.language?.startsWith('ru') ? ruRU : enUS;
  dayjs.locale(i18n.language?.startsWith('ru') ? 'ru' : 'en');

  return (
    <ConfigProvider
      theme={{
        algorithm: darkMode ? theme.darkAlgorithm : theme.defaultAlgorithm,
      }}
      locale={antdLocale}
    >
      {children}
    </ConfigProvider>
  );
};

const App = () => {
  const queryClient = new QueryClient();

  return (
    <Router>
      <ThemeProvider>
        <ThemedApp>
          <AuthProvider>
            <QueryClientProvider client={queryClient}>
              <AppRoutesWrapper />
            </QueryClientProvider>
          </AuthProvider>
        </ThemedApp>
      </ThemeProvider>
    </Router>
  );
};

export default App;
