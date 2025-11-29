using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Application.Api.AppServices.ScheduleSnapshots
{
    public class ScheduledCategoryService: IScheduledCategoryAppService
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
