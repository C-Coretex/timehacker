import React from 'react';
import { Route, Routes } from 'react-router-dom';
import AppRoutes from './routes/AppRoutes';
// !!!TODO
// import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import { Layout } from './components/Layout';

function App() {
  return (
    <Layout>
      <Routes>
        {AppRoutes.map((route, index) => {
          const { element, requireAuth, ...rest } = route;
          // return <Route key={index} {...rest} element={requireAuth ? <AuthorizeRoute {...rest} element={element} /> : element} />;
          return <Route key={index} {...rest} element={element} />;
        })}
      </Routes>
    </Layout>
  );
}

export default App;
