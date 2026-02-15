// src/api/types.ts
import type { Dayjs } from 'dayjs';

export interface FixedTaskReturnModel {
    id: string;
    name: string;
    description: string;
    priority: number;
    startTimestamp: string;
    endTimestamp: string;
    repeatingEntity: ReturnRepeatingEntityModel | null;
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
    startTimestamp: Dayjs;
    endTimestamp: Dayjs;
}

export interface FixedTaskDisplayModel {
    id: string;
    name: string;
    description: string;
    priority: number;
    startTimestamp: Dayjs;
    endTimestamp: Dayjs;
    repeatingEntity: ReturnRepeatingEntityModel | null;
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

export interface InputDayRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.DayRepeatingEntity;
    daysCountToRepeat: number;
}

export interface InputWeekRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.WeekRepeatingEntity;
    repeatsOn: number[]; // DayOfWeekEnum values (1-7)
}

export interface InputMonthRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.MonthRepeatingEntity;
    monthDayToRepeat: number; // 1-31
}

export interface InputYearRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.YearRepeatingEntity;
    yearDayToRepeat: number; // 1-366
}

export type InputRepeatingEntityType =
    | InputDayRepeatingEntityModel
    | InputWeekRepeatingEntityModel
    | InputMonthRepeatingEntityModel
    | InputYearRepeatingEntityModel;

// --- Return repeating entity models (from API) ---

export interface ReturnDayRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.DayRepeatingEntity;
    daysCountToRepeat: number;
}

export interface ReturnWeekRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.WeekRepeatingEntity;
    repeatsOn: number[];
}

export interface ReturnMonthRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.MonthRepeatingEntity;
    monthDayToRepeat: number;
}

export interface ReturnYearRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.YearRepeatingEntity;
    yearDayToRepeat: number;
}

export type ReturnRepeatingEntityModel =
    | ReturnDayRepeatingEntityModel
    | ReturnWeekRepeatingEntityModel
    | ReturnMonthRepeatingEntityModel
    | ReturnYearRepeatingEntityModel;

export interface EndsOnModel {
    maxDate?: string; // YYYY-MM-DD
    maxOccurrences?: number;
}

export interface InputScheduleEntityModel {
    parentEntityId: string; // Guid
    repeatingEntityType: InputRepeatingEntityType;
    endsOnModel?: EndsOnModel | null;
}