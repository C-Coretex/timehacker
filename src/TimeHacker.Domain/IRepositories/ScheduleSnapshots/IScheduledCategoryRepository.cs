using TimeHacker.Domain.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.IRepositories.ScheduleSnapshots;

public interface IScheduledCategoryRepository : IUserScopedRepositoryBase<ScheduledCategory, Guid>
{ }
