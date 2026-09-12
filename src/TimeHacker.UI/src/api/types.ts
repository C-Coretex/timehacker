// src/api/types.ts
import type { Dayjs } from 'dayjs';

export interface ScheduleEntityReturnModel {
    id: string;
    repeatingEntity: ReturnRepeatingEntityModel;
    scheduleCreated: string;  // ISO datetime
    lastTaskCreated: string | null;  // ISO date (YYYY-MM-DD)
    endsOn: string | null;  // ISO date (YYYY-MM-DD)
}

export interface FixedTaskReturnModel {
    id: string;
    name: string;
    description: string;
    priority: number;
    startTimestamp: string;
    endTimestamp: string;
    scheduleEntity: ScheduleEntityReturnModel | null;
    tags: unknown[];
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
    scheduleEntity: ScheduleEntityReturnModel | null;
    tags: unknown[];
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
    OnceRepeatingEntity: 5,
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

/** Applies only on an explicit list of dates rather than on a repeating pattern. */
export interface InputOnceRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.OnceRepeatingEntity;
    dates: string[]; // YYYY-MM-DD
}

export type InputRepeatingEntityType =
    | InputDayRepeatingEntityModel
    | InputWeekRepeatingEntityModel
    | InputMonthRepeatingEntityModel
    | InputYearRepeatingEntityModel
    | InputOnceRepeatingEntityModel;

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

export interface ReturnOnceRepeatingEntityModel {
    entityType: typeof RepeatingEntityTypeEnum.OnceRepeatingEntity;
    dates: string[]; // YYYY-MM-DD
}

export type ReturnRepeatingEntityModel =
    | ReturnDayRepeatingEntityModel
    | ReturnWeekRepeatingEntityModel
    | ReturnMonthRepeatingEntityModel
    | ReturnYearRepeatingEntityModel
    | ReturnOnceRepeatingEntityModel;

export interface EndsOnModel {
    maxDate?: string; // YYYY-MM-DD
    maxOccurrences?: number;
}

export interface InputScheduleEntityModel {
    parentEntityId: string; // Guid
    repeatingEntityType: InputRepeatingEntityType;
    endsOnModel?: EndsOnModel | null;
}

// --- Categories ---
// A category is a label (name/description/color) that tasks link to, and is what names its calendar bands.
// It owns many schedules, each of which is one dated time window (startTime/endTime on a `date`), exactly
// as a fixed task is anchored to its startTimestamp. A window always lands on its own date; its
// scheduleEntity, if present, repeats it on later days. One category may own several windows on the same
// day and they may overlap, so each window carries an optional `description` to tell them apart.
// Colors live on the category and travel as a signed ARGB int32 — see utils/colorArgb.

export interface CategoryReturnModel {
    id: string;
    name: string;
    description: string | null;
    color: number;
    schedules: CategoryScheduleReturnModel[];
}

export interface InputCategory {
    name: string;
    description?: string;
    color: number;
}

export interface CategoryFormData {
    name: string;
    description: string;
    color: number;
}

export interface CategoryDisplayModel {
    id: string;
    name: string;
    description: string | null;
    color: number;
    schedules: CategoryScheduleDisplayModel[];
}

// --- Category schedules ---

export interface CategoryScheduleReturnModel {
    id: string;
    categoryId: string;
    /** Optional note distinguishing this window from the category's others. */
    description: string | null;
    date: string; // YYYY-MM-DD
    startTime: string; // HH:mm:ss
    endTime: string; // HH:mm:ss
    scheduleEntity: ScheduleEntityReturnModel | null;
}

export interface InputCategorySchedule {
    description?: string;
    date: string; // YYYY-MM-DD
    startTime: string; // HH:mm:ss
    endTime: string; // HH:mm:ss
}

export interface CategoryScheduleFormData {
    description: string;
    date: Dayjs;
    startTime: Dayjs;
    endTime: Dayjs;
}

export interface CategoryScheduleDisplayModel {
    id: string;
    categoryId: string;
    description: string | null;
    date: Dayjs;
    startTime: Dayjs;
    endTime: Dayjs;
    scheduleEntity: ScheduleEntityReturnModel | null;
}