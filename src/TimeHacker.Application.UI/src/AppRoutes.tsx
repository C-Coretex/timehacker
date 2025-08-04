import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import CalendarPage from 'pages/CalendarPage';
import NotFoundPage from 'pages/NotFoundPage';
import AboutPage from 'pages/AboutPage';
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
        path: 'tasks',
        element: <TasksPage />,
      },
      {
        path: '*',
        element: <NotFoundPage />,
      },
    ]
  }
];

export default AppRoutes;
