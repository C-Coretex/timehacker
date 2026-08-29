using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Domain.Helpers.ScheduleSnapshots;

public static class ScheduleEntityHelper
{
    /// <summary>
    /// Builds a <see cref="ScheduleEntity"/> from a recurrence pattern, resolving the user's "ends on" choice
    /// into a single concrete <see cref="ScheduleEntity.EndsOn"/> date. A "max occurrences" limit has no fixed
    /// date, so it is expanded by walking the recurrence forward N times; if a "max date" is also given, the
    /// earlier of the two wins.
    /// </summary>
    /// <param name="anchorDate">
    /// The day the parent task/category already occupies on its own. That instance exists outside this
    /// recurrence, so it seeds both progress markers and the series resumes strictly after it — which is
    /// what keeps the anchor day from being generated a second time.
    /// </param>
    public static ScheduleEntity GetScheduleEntity(RepeatingEntityDto repeatingEntityModel, EndsOnModel? endsOnModel, DateOnly anchorDate, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(repeatingEntityModel);
        ArgumentNullException.ThrowIfNull(timeProvider);

        var scheduleEntity = new ScheduleEntity
        {
            RepeatingEntity = repeatingEntityModel,
            FirstEntityCreated = anchorDate,
            LastEntityCreated = anchorDate
        };

        // An explicit list of dates is already bounded, so it defines its own EndsOn and ignores the
        // caller's "ends on" choice entirely.
        if (repeatingEntityModel.RepeatingData is OnceRepeatingEntity once)
        {
            // The series only ever moves forward from the anchor, and never from before today, so a date
            // at or below that floor could never produce an occurrence — reject it instead of silently
            // dropping it.
            var floor = DateTimeHelpers.Max(anchorDate, DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime));
            if (once.Dates.Min() <= floor)
                throw new DataIsNotCorrectException($"Every chosen date must be after {floor:yyyy-MM-dd}.", nameof(repeatingEntityModel));

            scheduleEntity.EndsOn = once.Dates.Max();
        }

        if (endsOnModel == null)
            return scheduleEntity;

        if (endsOnModel.MaxOccurrences != null)
        {
            // Convert an occurrence count into a date by stepping the recurrence forward that many times
            // (stopping early if it would pass an explicit MaxDate). Counting starts at the anchor, since
            // that is where the series begins.
            var date = anchorDate;
            for (var i = 0; (i < endsOnModel.MaxOccurrences && date < endsOnModel.MaxDate.GetValueOrDefault(DateOnly.MaxValue)); i++)
            {
                if (repeatingEntityModel.RepeatingData.GetNextTaskDate(date) is not { } nextDate)
                    break;
                date = nextDate;
            }

            scheduleEntity.EndsOn = DateTimeHelpers.Min(scheduleEntity.EndsOn ?? DateOnly.MaxValue, date);
        }

        // Clamp to MaxDate: whichever of the occurrence-derived date and MaxDate is earlier.
        // If no MaxDate was given, this is a no-op.
        if (endsOnModel.MaxDate != null)
            scheduleEntity.EndsOn = DateTimeHelpers.Min(scheduleEntity.EndsOn ?? DateOnly.MaxValue, endsOnModel.MaxDate.Value);

        return scheduleEntity;
    }
}
