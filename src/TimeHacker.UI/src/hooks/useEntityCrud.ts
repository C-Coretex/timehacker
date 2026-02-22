import { useState, useCallback, useEffect, useRef } from 'react';
import { notification } from 'antd';

interface UseEntityCrudOptions<TDisplay> {
  fetchFn: () => Promise<TDisplay[]>;
  fetchErrorMessage: string;
}

interface UseEntityCrudResult<TDisplay> {
  items: TDisplay[];
  loading: boolean;
  error: string | null;
  fetch: () => Promise<void>;
  withRefetch: (action: () => Promise<unknown>, errorMessage: string) => Promise<void>;
}

/**
 * Manages fetch/loading/error state for a list of entities and provides a helper
 * to run a mutating action then refetch.
 */
export function useEntityCrud<TDisplay>({
  fetchFn,
  fetchErrorMessage,
}: UseEntityCrudOptions<TDisplay>): UseEntityCrudResult<TDisplay> {
  const [items, setItems] = useState<TDisplay[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Refs keep the latest values without adding them as deps of `fetch`
  const fetchFnRef = useRef(fetchFn);
  fetchFnRef.current = fetchFn;
  const errorMsgRef = useRef(fetchErrorMessage);
  errorMsgRef.current = fetchErrorMessage;

  const fetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await fetchFnRef.current());
    } catch {
      const msg = errorMsgRef.current;
      setError(msg);
      notification.error({ message: 'Error', description: msg });
    } finally {
      setLoading(false);
    }
  }, []);

  const withRefetch = useCallback(
    async (action: () => Promise<unknown>, errorMessage: string) => {
      try {
        await action();
        await fetch();
      } catch {
        notification.error({ message: 'Error', description: errorMessage });
      }
    },
    [fetch]
  );

  useEffect(() => {
    fetch();
  }, [fetch]);

  return { items, loading, error, fetch, withRefetch };
}
