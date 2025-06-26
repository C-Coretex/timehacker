using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.Services.ScheduleSnapshots
{
    public class ScheduledCategoryService: IScheduledCategoryService
    {
        private readonly IScheduledCategoryRepository _scheduledCategoryRepository;
        private readonly UserAccessorBase _userAccessorBase;
        public ScheduledCategoryService(IScheduledCategoryRepository scheduledCategoryRepository, UserAccessorBase userAccessorBase)
        {
            _scheduledCategoryRepository = scheduledCategoryRepository;
            _userAccessorBase = userAccessorBase;
        }
    }
}
