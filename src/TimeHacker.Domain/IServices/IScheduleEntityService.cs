namespace TimeHacker.Domain.IServices;

public interface IScheduleEntityService
{
    /// <summary>Active recurrences whose parent is a <see cref="FixedTask"/>.</summary>
    IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from);

    /// <summary>Active recurrences whose parent is a <see cref="Category"/>.</summary>
    IQueryable<ScheduleEntityReturn> GetAllCategoriesFrom(DateOnly from);

    Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated, CancellationToken cancellationToken = default);
}
