import { BrowserRouter as Router, useRoutes } from 'react-router-dom';
import { ThemeProvider } from 'contexts/ThemeContext';
import AppRoutes from 'config/AppRoutes';

const AppRoutesWrapper = () => {
  const element = useRoutes(AppRoutes);
  return element;
};

const App = () => {
  return (
    <Router>
      <ThemeProvider>
        <AppRoutesWrapper />
      </ThemeProvider>
    </Router>
  );
};

export default App;
