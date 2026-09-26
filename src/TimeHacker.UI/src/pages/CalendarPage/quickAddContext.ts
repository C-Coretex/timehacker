import { createContext } from 'react';

/**
 * Opens the task form for the hour starting at the given time. Provided by PlannerCalendar to the "+" buttons
 * that QuickAddSlot renders — rbc builds the slots itself, so a prop cannot reach them.
 */
export const QuickAddContext = createContext<((hourStart: Date) => void) | null>(null);
