using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.IRepositories.ScheduleSnapshots
{
    public interface IScheduledCategoryRepository : IRepositoryBase<ScheduledCategory, ulong>
    { }
}
