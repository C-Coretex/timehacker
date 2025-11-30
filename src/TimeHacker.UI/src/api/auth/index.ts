import axios from 'config/axios';
import type { AxiosResponse } from 'axios';
import type { LoginRequestProps, LoginRequestResponse } from 'types/auth';

export const loginUser = async (
  data: LoginRequestProps
): Promise<AxiosResponse<LoginRequestResponse>> => {
  return await axios.post<LoginRequestResponse>(
    '/login?useCookies=true&useSessionCookies=true',
    data
  );
};
