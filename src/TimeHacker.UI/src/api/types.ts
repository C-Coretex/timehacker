// src/api/types.ts
import type moment from 'moment';

export interface FixedTaskReturnModel {
    id: string;
    name: string;
    description: string;
    priority: number;
    startTimestamp: string;
    endTimestamp: string;
}

export interface InputFixedTask {
    name: string;
    description: string;
    priority: number;
    startTimestamp: string;
    endTimestamp: string;
}

export interface FixedTaskFormData {
    name: string;
    description: string;
    priority: number;
    startTimestamp: moment.Moment;
    endTimestamp: moment.Moment;
}

export interface FixedTaskDisplayModel {
    id: string;
    name: string;
    description: string;
    priority: number;
    startTimestamp: moment.Moment;
    endTimestamp: moment.Moment;
}

export interface DynamicTaskReturnModel {
    id: string;
    name: string;
    description: string | null;
    priority: number;
    minTimeToFinish: string;
    maxTimeToFinish: string;
    optimalTimeToFinish: string | null;
    createdTimestamp: string;
    tags: unknown[];
}

export interface InputDynamicTask {
    name: string;
    description?: string;
    categoryIds?: string[];
    priority: number;
    minTimeToFinish: string;
    maxTimeToFinish: string;
    optimalTimeToFinish?: string;
}

// --- Scheduled entities (repeating schedule for fixed tasks) ---

export const RepeatingEntityTypeEnum = {
    DayRepeatingEntity: 1,
    WeekRepeatingEntity: 2,
    MonthRepeatingEntity: 3,
    YearRepeatingEntity: 4,
} as const;
export type RepeatingEntityTypeEnum = (typeof RepeatingEntityTypeEnum)[keyof typeof RepeatingEntityTypeEnum];

export const DayOfWeekEnum = {
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6,
    Sunday: 7,
} as const;
export type DayOfWeekEnum = (typeof DayOfWeekEnum)[keyof typeof DayOfWeekEnum];

/** API expects entityType as string enum name, e.g. "DayRepeatingEntity" */
export type RepeatingEntityTypeName =
    | 'DayRepeatingEntity'
    | 'WeekRepeatingEntity'
    | 'MonthRepeatingEntity'
    | 'YearRepeatingEntity';

export interface InputDayRepeatingEntityModel {
    entityType: RepeatingEntityTypeName;
    daysCountToRepeat: number;
}

export interface InputWeekRepeatingEntityModel {
    entityType: RepeatingEntityTypeName;
    repeatsOn: number[]; // DayOfWeekEnum values (1-7)
}

export interface InputMonthRepeatingEntityModel {
    entityType: RepeatingEntityTypeName;
    monthDayToRepeat: number; // 1-31
}

export interface InputYearRepeatingEntityModel {
    entityType: RepeatingEntityTypeName;
    yearDayToRepeat: number; // 1-366
}

export type InputRepeatingEntityType =
    | InputDayRepeatingEntityModel
    | InputWeekRepeatingEntityModel
    | InputMonthRepeatingEntityModel
    | InputYearRepeatingEntityModel;

export interface EndsOnModel {
    maxDate?: string; // YYYY-MM-DD
    maxOccurrences?: number;
}

export interface InputScheduleEntityModel {
    parentEntityId: string; // Guid
    repeatingEntityType: InputRepeatingEntityType;
    endsOnModel?: EndsOnModel | null;
}