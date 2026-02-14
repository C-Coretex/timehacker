import type { FC } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { Spin } from 'antd';
import { useAuth } from 'contexts/AuthContext';
import type { PrivateRouteProps } from './types';

const PrivateRoute: FC<PrivateRouteProps> = ({
  auth = true,
  // roles = [],
  // permissions = [],
}) => {
  const { isAuthenticated, loading } = useAuth();

  if (auth && loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (auth && !isAuthenticated) {
    return <Navigate to="/login" state={{ message: 'Please log in to access this page' }} replace />;
  }

  // if (auth && !hasAccess(roles, permissions)) {
  //   return <UnauthorizedPage />;
  // }

  return <Outlet />;
};

export default PrivateRoute;
