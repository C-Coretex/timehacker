import { useState, useCallback, useEffect, useRef } from 'react';
import { App } from 'antd';
import { useTranslation } from 'react-i18next';

interface UseEntityCrudOptions<TDisplay> {
  fetchFn: () => Promise<TDisplay[]>;
  fetchErrorMessage: string;
}

/** Outcome of a mutation run through `withRefetch`; failures were already reported to the user. */
export type MutationResult<T> = { succeeded: true; value: T } | { succeeded: false };

interface UseEntityCrudResult<TDisplay> {
  items: TDisplay[];
  loading: boolean;
  error: string | null;
  fetch: () => Promise<void>;
  withRefetch: <T>(action: () => Promise<T>, errorMessage: string) => Promise<MutationResult<T>>;
}

/**
 * Manages fetch/loading/error state for a list of entities and provides a helper
 * to run a mutating action then refetch. The helper reports the outcome instead of throwing, so callers
 * can skip their success message (and keep a dialog open) when the action failed.
 */
export function useEntityCrud<TDisplay>({
  fetchFn,
  fetchErrorMessage,
}: UseEntityCrudOptions<TDisplay>): UseEntityCrudResult<TDisplay> {
  const { notification } = App.useApp();
  const { t } = useTranslation();
  const [items, setItems] = useState<TDisplay[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Refs keep the latest values without adding them as deps of `fetch`
  const fetchFnRef = useRef(fetchFn);
  fetchFnRef.current = fetchFn;
  const errorMsgRef = useRef(fetchErrorMessage);
  errorMsgRef.current = fetchErrorMessage;
  const tRef = useRef(t);
  tRef.current = t;

  const fetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await fetchFnRef.current());
    } catch {
      const msg = errorMsgRef.current;
      setError(msg);
      notification.error({ title: tRef.current('errors.generic'), description: msg });
    } finally {
      setLoading(false);
    }
  }, [notification]);

  const withRefetch = useCallback(
    async <T,>(action: () => Promise<T>, errorMessage: string): Promise<MutationResult<T>> => {
      try {
        const value = await action();
        await fetch();
        return { succeeded: true, value };
      } catch {
        notification.error({ title: tRef.current('errors.generic'), description: errorMessage });
        return { succeeded: false };
      }
    },
    [fetch, notification]
  );

  useEffect(() => {
    fetch();
  }, [fetch]);

  return { items, loading, error, fetch, withRefetch };
}
