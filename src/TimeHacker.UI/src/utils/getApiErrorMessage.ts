/**
 * Extracts a human-readable message from an Axios-style error, if the backend
 * provided one in `response.data.message`. Returns null when unavailable so
 * callers can fall back to a localized default.
 */
export function getApiErrorMessage(err: unknown): string | null {
  if (err && typeof err === 'object' && 'response' in err) {
    const response = (err as { response?: { data?: { message?: string } } }).response;
    return response?.data?.message ?? null;
  }
  return null;
}
