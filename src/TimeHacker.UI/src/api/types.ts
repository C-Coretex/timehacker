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