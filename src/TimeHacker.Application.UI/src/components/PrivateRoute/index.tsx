import type { FC } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import type { PrivateRouteProps } from './types';
import UnauthorizedPage from 'pages/UnauthorizedPage';

// test data (later will be fetched from backend)
const authenticated = true;
const userInfo = {
  roles: ['user', 'admin'],
  permissions: ['create_task'],
};

const hasAccess = (
  requiredRoles: string[],
  requiredPermissions: string[]
): boolean => {
  const roleMatch = requiredRoles.some((role) => userInfo.roles.includes(role));
  const permissionMatch = requiredPermissions.some((p) =>
    userInfo.permissions.includes(p)
  );
  return roleMatch || permissionMatch;
};

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
