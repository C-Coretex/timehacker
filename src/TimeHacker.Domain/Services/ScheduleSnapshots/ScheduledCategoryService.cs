using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduledCategoryService: IScheduledCategoryService
    {
        private readonly IScheduledCategoryRepository _scheduledCategoryRepository;
        private readonly IUserAccessor _userAccessor;
        public ScheduledCategoryService(IScheduledCategoryRepository scheduledCategoryRepository, IUserAccessor userAccessor)
        {
            _scheduledCategoryRepository = scheduledCategoryRepository;
            _userAccessor = userAccessor;
        }
    }
}
