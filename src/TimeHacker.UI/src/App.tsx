import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import { ConfigProvider, theme } from 'antd';
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

  return (
    <ConfigProvider
      theme={{
        algorithm: darkMode ? theme.darkAlgorithm : theme.defaultAlgorithm,
      }}
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
