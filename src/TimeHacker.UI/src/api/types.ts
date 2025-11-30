// src/api/types.ts
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