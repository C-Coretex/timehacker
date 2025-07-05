import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import CalendarPage from 'pages/CalendarPage';
import NotFoundPage from 'pages/NotFoundPage';

const AppRoutes: RouteObject[] = [
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        index: true,
        element: <CalendarPage />,
      },
      {
        path: '*',
        element: <NotFoundPage />,
      },
    ],
  },
];

export default AppRoutes;
