export interface ApiResponse<T = unknown> {
  success: boolean;
  data: T;
  errors?: Record<string, string[]>;
}
