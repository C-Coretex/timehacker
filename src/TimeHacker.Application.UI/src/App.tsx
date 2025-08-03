import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
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
        <QueryClientProvider client={queryClient}>
          <AppRoutesWrapper />
        </QueryClientProvider>
      </ThemeProvider>
    </Router>
  );
};

export default App;
