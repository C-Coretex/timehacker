import { useRoutes } from 'react-router-dom';
import AppRoutes from './AppRoutes';

const App = () => {
  const element = useRoutes(AppRoutes);
  return element;
};

export default App;
