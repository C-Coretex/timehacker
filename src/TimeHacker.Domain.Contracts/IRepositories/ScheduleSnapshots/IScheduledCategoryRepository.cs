using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots
{
    public interface IScheduledCategoryRepository : IRepositoryBase<ScheduledCategory, ulong>
    { }
}
