using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduledCategoryService: IScheduledCategoryService
    {
        private readonly IScheduledCategoryRepository _scheduledCategoryRepository;
        public ScheduledCategoryService(IScheduledCategoryRepository scheduledCategoryRepository)
        {
            _scheduledCategoryRepository = scheduledCategoryRepository;
        }


    }
}
