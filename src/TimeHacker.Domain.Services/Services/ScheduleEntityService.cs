using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.Services;

public class ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository) : IScheduleEntityService
{
    public IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from)
    {
        var query = scheduleEntityRepository.GetAll().Where(x => x.FixedTask != null)
            .Where(x => x.EndsOn == null || x.EndsOn >= from);

        return query.Select(scheduleEntity => new ScheduleEntityReturn()
        {
            Id = scheduleEntity.Id,
            UserId = scheduleEntity.UserId,
            RepeatingEntity = scheduleEntity.RepeatingEntity,
            CreatedTimestamp = scheduleEntity.CreatedTimestamp,
            FirstEntityCreated = scheduleEntity.FirstEntityCreated,
            LastEntityCreated = scheduleEntity.LastEntityCreated,
            EndsOn = scheduleEntity.EndsOn,
            ScheduledTasks = scheduleEntity.ScheduledTasks,
            ScheduledCategories = scheduleEntity.ScheduledCategories,
            FixedTask = scheduleEntity.FixedTask
        });
    }

    /// <summary>
    /// Advances a schedule's recurrence-progress marker after a task has been generated for
    /// <paramref name="entityCreated"/>. This is what lets generation resume incrementally instead of
    /// re-expanding the whole recurrence each time (see <see cref="ScheduleEntityReturn.GetNextEntityDatesIn"/>).
    /// </summary>
    public async Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated, CancellationToken cancellationToken = default)
    {
        var scheduleEntity = await scheduleEntityRepository.GetByIdAsync(id, asNoTracking: false, cancellationToken);
        if (scheduleEntity == null)
            return;

        // Guard against marking a date that the recurrence pattern would never actually produce.
        var scheduleEntityReturn = ScheduleEntityReturn.Create(scheduleEntity);
        if (!scheduleEntityReturn.IsEntityDateCorrect(entityCreated))
            throw new DataIsNotCorrectException("Created entity timestamp is not correct", nameof(entityCreated));

        // The marker only ever moves forward; ignore out-of-order/backfill updates.
        if (scheduleEntity.LastEntityCreated != null && scheduleEntity.LastEntityCreated >= entityCreated)
            return;

        scheduleEntity.LastEntityCreated = entityCreated;
        // Seed the lower bound on first generation so future recalculations know where the series began.
        if (scheduleEntity.FirstEntityCreated == null)
            scheduleEntity.FirstEntityCreated = scheduleEntity.LastEntityCreated;

        await scheduleEntityRepository.SaveChangesAsync(cancellationToken);
    }
}
