import dayjs from 'dayjs';
import type { Dayjs } from 'dayjs';
import { RepeatingEntityTypeEnum } from '../api/types';
import type { EndsOnModel, InputRepeatingEntityType } from '../api/types';

export interface SchedulePayload {
  repeatingEntityType: InputRepeatingEntityType;
  endsOnModel?: EndsOnModel | null;
}

/**
 * Turns the flat field values owned by `ScheduleFormSection` into a schedule request body.
 * Shared by the task and category modals — both render that same section inside their own Form.
 */
export function buildSchedulePayload(values: Record<string, unknown>): SchedulePayload | undefined {
  if (!values.addSchedule || values.scheduleType == null) return undefined;

  let repeatingEntityType: InputRepeatingEntityType;
  switch (values.scheduleType as RepeatingEntityTypeEnum) {
    case RepeatingEntityTypeEnum.DayRepeatingEntity:
      repeatingEntityType = {
        entityType: RepeatingEntityTypeEnum.DayRepeatingEntity,
        daysCountToRepeat: (values.daysCountToRepeat as number) ?? 1,
      };
      break;
    case RepeatingEntityTypeEnum.WeekRepeatingEntity:
      repeatingEntityType = {
        entityType: RepeatingEntityTypeEnum.WeekRepeatingEntity,
        repeatsOn: (values.repeatsOn as number[]) ?? [],
      };
      break;
    case RepeatingEntityTypeEnum.MonthRepeatingEntity:
      repeatingEntityType = {
        entityType: RepeatingEntityTypeEnum.MonthRepeatingEntity,
        monthDayToRepeat: (values.monthDayToRepeat as number) ?? 1,
      };
      break;
    case RepeatingEntityTypeEnum.YearRepeatingEntity:
      repeatingEntityType = {
        entityType: RepeatingEntityTypeEnum.YearRepeatingEntity,
        yearDayToRepeat: (values.yearDayToRepeat as number) ?? 1,
      };
      break;
    case RepeatingEntityTypeEnum.OnceRepeatingEntity: {
      const dates = (values.onceDates as Dayjs[] | undefined) ?? [];
      if (dates.length === 0) return undefined;
      return {
        // A finite list of dates defines its own end, so the server derives EndsOn and the
        // "ends on" fields are not sent.
        repeatingEntityType: {
          entityType: RepeatingEntityTypeEnum.OnceRepeatingEntity,
          dates: dates.map((d) => dayjs(d).format('YYYY-MM-DD')),
        },
        endsOnModel: null,
      };
    }
    default:
      return undefined;
  }

  const endsOnModel: EndsOnModel | undefined =
    values.endsOnMaxDate || values.endsOnMaxOccurrences != null
      ? {
          maxDate: values.endsOnMaxDate
            ? dayjs(values.endsOnMaxDate as Dayjs).format('YYYY-MM-DD')
            : undefined,
          maxOccurrences:
            (values.endsOnMaxOccurrences as number) > 0
              ? (values.endsOnMaxOccurrences as number)
              : undefined,
        }
      : undefined;

  return { repeatingEntityType, endsOnModel: endsOnModel ?? null };
}
