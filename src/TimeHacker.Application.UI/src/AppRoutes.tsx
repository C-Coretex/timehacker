import ProtectedRoute from 'components/ProtectedRoute';
import type { RouteObject } from 'react-router-dom';

import Layout from 'components/Layout';
import CalendarPage from 'pages/CalendarPage';
import NotFoundPage from 'pages/NotFoundPage';
import AboutPage from 'pages/AboutPage';
import TasksPage from 'pages/TasksPage';
import LoginPage from 'pages/LoginPage';

const isAuthenticated = false;

const AppRoutes: RouteObject[] = [
  {
    path: '/',
    element: <Layout />,
    children: [
        {
            index: true,
            element: (
                <ProtectedRoute isAuthenticated={isAuthenticated}>
                    <CalendarPage />
                </ProtectedRoute>
            ),
        },
        { path: 'login', element: <LoginPage /> },
        { path: 'about', element: <AboutPage /> },
        {
            path: 'tasks',
            element: (
                <ProtectedRoute isAuthenticated={isAuthenticated}>
                    <TasksPage />
                </ProtectedRoute>
            ),
        },
        { path: '*', element: <NotFoundPage /> },
    ]
  }
];

export default AppRoutes;
