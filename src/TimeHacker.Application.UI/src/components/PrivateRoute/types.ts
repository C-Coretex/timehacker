type AuthWithAccess = {
  auth: true; // must be true if roles or permissions given
  roles?: string[];
  permissions?: string[];
};

type AuthWithoutAccess = {
  auth?: false; // can be false only if roles and permissions NOT present
  roles?: never;
  permissions?: never;
};

export type PrivateRouteProps = AuthWithAccess | AuthWithoutAccess;
