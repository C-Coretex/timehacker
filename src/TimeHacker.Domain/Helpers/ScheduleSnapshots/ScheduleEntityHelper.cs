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
    public static ScheduleEntity GetScheduleEntity(RepeatingEntityDto repeatingEntityModel, EndsOnModel? endsOnModel, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(repeatingEntityModel);
        ArgumentNullException.ThrowIfNull(timeProvider);

        var scheduleEntity = new ScheduleEntity
        {
            RepeatingEntity = repeatingEntityModel
        };

        if (endsOnModel == null)
            return scheduleEntity;

        if (endsOnModel.MaxOccurrences != null)
        {
            // Convert an occurrence count into a date by stepping the recurrence forward that many times
            // (stopping early if it would pass an explicit MaxDate).
            var date = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
            for (var i = 0; (i < endsOnModel.MaxOccurrences && date < endsOnModel.MaxDate.GetValueOrDefault(DateOnly.MaxValue)); i++)
                date = repeatingEntityModel.RepeatingData.GetNextTaskDate(date);

            scheduleEntity.EndsOn = date;
        }

        // Clamp to MaxDate: whichever of the occurrence-derived date and MaxDate is earlier.
        // If no MaxDate was given, this is a no-op.
        if (scheduleEntity.EndsOn == null || scheduleEntity.EndsOn > endsOnModel.MaxDate)
            scheduleEntity.EndsOn = endsOnModel.MaxDate;

        return scheduleEntity;
    }
}
