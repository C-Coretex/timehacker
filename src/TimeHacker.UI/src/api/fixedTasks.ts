// src/api/fixedTasks.ts
import type {
    FixedTaskReturnModel,
    InputFixedTask,
    InputScheduleEntityModel,
} from './types';
import api from './api';

const API_BASE_URL = '/api/fixed-tasks';
const TASKS_API_URL = '/api/tasks';

export const fetchFixedTasks = async (): Promise<FixedTaskReturnModel[]> => {
    const response = await api.get(`${API_BASE_URL}`);
    return response.data;
};

export const fetchFixedTaskById = async (id: string): Promise<FixedTaskReturnModel> => {
    const response = await api.get(`${API_BASE_URL}/${id}`);
    return response.data;
};

/** Add fixed task. Returns the new task's Id (Guid). */
export const createFixedTask = async (task: InputFixedTask): Promise<string> => {
    const response = await api.post<string>(`${API_BASE_URL}`, task);
    return response.data;
};

/** Build request body with PascalCase keys for API (converter expects EntityType discriminator). */
function toScheduleRequestBody(body: InputScheduleEntityModel): Record<string, unknown> {
    const rep = body.repeatingEntityType;
    const repeatingEntityType =
        'daysCountToRepeat' in rep
            ? { EntityType: rep.entityType, DaysCountToRepeat: rep.daysCountToRepeat }
            : 'repeatsOn' in rep
              ? { EntityType: rep.entityType, RepeatsOn: rep.repeatsOn }
              : 'monthDayToRepeat' in rep
                ? { EntityType: rep.entityType, MonthDayToRepeat: rep.monthDayToRepeat }
                : { EntityType: rep.entityType, YearDayToRepeat: rep.yearDayToRepeat };
    return {
        parentEntityId: body.parentEntityId,
        repeatingEntityType,
        endsOnModel: body.endsOnModel
            ? {
                  MaxDate: body.endsOnModel.maxDate ?? undefined,
                  MaxOccurrences: body.endsOnModel.maxOccurrences ?? undefined,
              }
            : undefined,
    };
}

/** Post new schedule for a fixed task (repeating entity). Call after createFixedTask with the returned id. */
export const postNewScheduleForTask = async (
    body: InputScheduleEntityModel
): Promise<unknown> => {
    const payload = toScheduleRequestBody(body);
    const response = await api.post(`${TASKS_API_URL}/schedules`, payload);
    return response.data;
};

export const updateFixedTask = async (id: string, task: InputFixedTask): Promise<void> => {
    await api.put(`${API_BASE_URL}/${id}`, task);
};

export const deleteFixedTask = async (id: string): Promise<void> => {
    await api.delete(`${API_BASE_URL}/${id}`);
};
