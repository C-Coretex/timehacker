import { useMutation as useTanstackMutation } from '@tanstack/react-query';
import type { ApiResponse } from 'types/api';

export const useMutation = <TData = unknown, TVariables = void>(
  mutationFn: (variables: TVariables) => Promise<ApiResponse<TData>>,
  options = {}
) => {
  return useTanstackMutation<ApiResponse<TData>, Error, TVariables, unknown>({
    mutationFn,
    ...options,
  });
};
