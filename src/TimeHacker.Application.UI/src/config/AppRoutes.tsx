import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import PrivateRoute from 'components/PrivateRoute';
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
        element: <PrivateRoute auth={true} roles={['admin']} />,
        children: [
          {
            path: 'protected',
            element: <div>Protected page</div>,
          },
          {
            path: 'other-protected',
            element: <div>Other protected page</div>,
          },
        ],
      },
      {
        path: '*',
        element: <NotFoundPage />,
      },
    ],
  },
];

export default AppRoutes;
