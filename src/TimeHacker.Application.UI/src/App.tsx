import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import { AuthProvider } from 'contexts/AuthContext';
import { ThemeProvider } from 'contexts/ThemeContext';
import AppRoutes from 'config/AppRoutes';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

const AppRoutesWrapper = () => {
  const element = useRoutes(AppRoutes);
  return element;
};

const App = () => {
  const queryClient = new QueryClient();

  return (
    <Router>
      <ThemeProvider>
        <AuthProvider>
          <QueryClientProvider client={queryClient}>
            <AppRoutesWrapper />
          </QueryClientProvider>
        </AuthProvider>
      </ThemeProvider>
    </Router>
  );
};

export default App;
