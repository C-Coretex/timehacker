import { useCallback, useEffect, useState } from 'react';
import { fetchFixedTasks } from '../api/fixedTasks';
import { fetchDynamicTasks } from '../api/dynamicTasks';
import type { CategoryReturnModel } from '../api/types';

/**
 * Maps task id to the categories that task is linked to.
 *
 * The calendar's timeline is served from a snapshot, which denormalizes only the fields needed to draw the
 * plan and carries no category links — but it does report each entry's originating task id, so the links
 * are joined on here from the task endpoints. A side effect worth knowing: these are the task's *current*
 * categories, so re-tagging a task shows up immediately rather than waiting for the snapshot to regenerate.
 */
export function useTaskCategories() {
  const [categoriesByTaskId, setCategoriesByTaskId] = useState<Map<string, CategoryReturnModel[]>>(
    new Map()
  );

  const fetchTaskCategories = useCallback(async () => {
    const [fixed, dynamic] = await Promise.all([fetchFixedTasks(), fetchDynamicTasks()]);
    setCategoriesByTaskId(
      new Map([...fixed, ...dynamic].map((task) => [task.id, task.categories ?? []]))
    );
  }, []);

  useEffect(() => {
    // A failure here only costs the colour dots, so the calendar itself still renders.
    void fetchTaskCategories().catch(() => setCategoriesByTaskId(new Map()));
  }, [fetchTaskCategories]);

  return { categoriesByTaskId, fetchTaskCategories };
}
