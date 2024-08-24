using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduleEntityService : IScheduleEntityService
    {
        private readonly IScheduleEntityRepository _scheduleEntityRepository;

        private readonly IUserAccessor _userAccessor;

        public ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository, IUserAccessor userAccessor)
        {
            _scheduleEntityRepository = scheduleEntityRepository;
            _userAccessor = userAccessor;
        }


    }
}
