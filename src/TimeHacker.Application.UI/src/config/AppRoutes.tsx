import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import PrivateRoute from 'components/PrivateRoute';
import AboutPage from 'pages/AboutPage';
import CalendarPage from 'pages/CalendarPage';
import LoginPage from 'pages/LoginPage';
import NotFoundPage from 'pages/NotFoundPage';
import TasksPage from 'pages/TasksPage';

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
        path: 'about',
        element: <AboutPage />,
      },
      {
        element: <PrivateRoute auth={true} />,
        children: [
          {
            path: 'tasks',
            element: <TasksPage />,
          },
        ],
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
  {
    path: 'login',
    element: <LoginPage />,
  },
];

export default AppRoutes;
