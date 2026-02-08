namespace TimeHacker.Domain.IServices;

public interface IScheduleEntityService
{
    IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from);

    Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated, CancellationToken cancellationToken = default);
}
