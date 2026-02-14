import type { FC } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import type { PrivateRouteProps } from './types';

// test data (later will be fetched from backend)
const authenticated = true;

const PrivateRoute: FC<PrivateRouteProps> = ({
  auth = true,
  // roles = [],
  // permissions = [],
}) => {
  //const { isAuthenticated } = useAuth();

  if (auth && !authenticated) {
    return <Navigate to="/login" replace />;
  }

  // if (auth && !hasAccess(roles, permissions)) {
  //   return <UnauthorizedPage />;
  // }

  return <Outlet />;
};

export default PrivateRoute;
