using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
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
