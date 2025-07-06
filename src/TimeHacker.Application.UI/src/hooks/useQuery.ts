import { useQuery as useTanstackQuery } from '@tanstack/react-query';
import type { ApiResponse } from 'types/api';

export const useQuery = <TData = unknown>(
  key: unknown[],
  queryFn: () => Promise<ApiResponse<TData>>,
  options = {}
) => {
  return useTanstackQuery<ApiResponse<TData>, Error, TData>({
    queryKey: key,
    queryFn,
    select: (res) => res.data,
    ...options,
  });
};
