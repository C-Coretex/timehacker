import type { FC } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { Spin } from 'antd';
import { useTranslation } from 'react-i18next';
import { useAuth } from 'contexts/AuthContext';
import type { PrivateRouteProps } from './types';

export const PrivateRoute: FC<PrivateRouteProps> = ({
  auth = true,
  // roles = [],
  // permissions = [],
}) => {
  const { isAuthenticated, loading } = useAuth();
  const { t } = useTranslation();

  if (auth && loading) {
    return (
      // Fills the shell's page panel (a flex column); 100vh would overflow it under the phone top bar.
      <div style={{ display: 'flex', flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <Spin size="large" />
      </div>
    );
  }

  if (auth && !isAuthenticated) {
    return <Navigate to="/login" state={{ message: t('privateRoute.loginRequired') }} replace />;
  }

  // if (auth && !hasAccess(roles, permissions)) {
  //   return <UnauthorizedPage />;
  // }

  return <Outlet />;
};

