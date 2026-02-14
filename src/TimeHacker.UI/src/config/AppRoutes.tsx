import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import PrivateRoute from 'components/PrivateRoute';
import AboutPage from 'pages/AboutPage';
import CalendarPage from 'pages/CalendarPage';
import LoginPage from 'pages/LoginPage';
import NotFoundPage from 'pages/NotFoundPage';
import ProfilePage from 'pages/ProfilePage';
import SettingsPage from 'pages/SettingsPage';
import TasksPage from 'pages/TasksPage';

const AppRoutes: RouteObject[] = [
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: 'about',
        element: <AboutPage />,
      },
      {
        path: 'settings',
        element: <SettingsPage />,
      },
      {
        element: <PrivateRoute auth={true} />,
        children: [
          {
            index: true,
            element: <CalendarPage />,
          },
          {
            path: 'tasks',
            element: <TasksPage />,
          },
          {
            path: 'profile',
            element: <ProfilePage />,
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
