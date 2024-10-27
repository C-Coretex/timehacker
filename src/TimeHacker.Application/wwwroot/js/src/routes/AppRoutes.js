// !!!TODO
// import ApiAuthorzationRoutes from '../components/api-authorization/ApiAuthorizationRoutes';

import { CalendarPage } from "../pages/CalendarPage";
import { NotFoundPage } from "../pages/NotFoundPage";

const AppRoutes = [
  {
    index: true,
    element: <CalendarPage />
  },
  {
    path: '/protected-page',
    requireAuth: true,
    element: <>I am protected</>
  },
  {
    path: '*',
    element: <NotFoundPage />
  },
  // ...ApiAuthorzationRoutes
];

export default AppRoutes;
