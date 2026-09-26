import type { TFunction } from 'i18next';

// Mirrors PriorityConstants on the server: 1 is the most important, and the scheduler favours lower numbers.
export const PRIORITY_HIGHEST = 1;
export const PRIORITY_LOWEST = 5;
export const PRIORITY_DEFAULT = 3;

const LEVEL_KEYS = ['highest', 'high', 'normal', 'low', 'lowest'] as const;
type PriorityLevelKey = (typeof LEVEL_KEYS)[number];

const levelKey = (priority: number): PriorityLevelKey => {
  const clamped = Math.min(PRIORITY_LOWEST, Math.max(PRIORITY_HIGHEST, Math.round(priority)));
  return LEVEL_KEYS[clamped - PRIORITY_HIGHEST];
};

export const priorityLabel = (priority: number, t: TFunction): string => t(`priority.${levelKey(priority)}`);

/** The level's `--th-priority-*` colour, for inline `style` use. */
export const priorityColor = (priority: number): string => `var(--th-priority-${levelKey(priority)})`;

export const isHighPriority = (priority: number): boolean => priority <= PRIORITY_HIGHEST + 1;

/** The two ends of the scale are marked wherever tasks are listed: Highest is ringed red, Lowest fades. */
export const priorityEmphasis = (priority: number): 'highest' | 'lowest' | undefined => {
  const key = levelKey(priority);
  return key === 'highest' || key === 'lowest' ? key : undefined;
};
