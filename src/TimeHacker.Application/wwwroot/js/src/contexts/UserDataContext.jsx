import React, { createContext, useEffect } from 'react';
import useQueryApiClient from '../utils/useQueryApiClient';
// !!!TODO
// import authService from '../components/api-authorization/AuthorizeService'

// Create a context
const UserDataContext = createContext();

// Create a provider component
const UserDataProvider = ({ children }) => {

  useEffect(() => {
    fetchUserData()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const fetchUserData = async () => {
    // const isAuthenticated = await authService.isAuthenticated()
    const isAuthenticated = false

    if (isAuthenticated) {
      refetch()
    }
  }

  const { data: userData, refetch } = useQueryApiClient({
    request: {
      url: 'api/user-data',
      method: 'GET',
      disableOnMount: true
    }
  });

  return (
    <UserDataContext.Provider value={{data: userData?.data, refetch}}>
      {children}
    </UserDataContext.Provider>
  );
};

export { UserDataContext, UserDataProvider };