using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduledTaskService: IScheduledTaskService
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepository;

        private readonly IUserAccessor _userAccessor;

        public ScheduledTaskService(IScheduledTaskRepository scheduledTaskRepository, IUserAccessor userAccessor)
        {
            _scheduledTaskRepository = scheduledTaskRepository;
            _userAccessor = userAccessor;
        }

        public async Task<ScheduledTask?> GetBy(Guid id)
        {
            var scheduledTask = await _scheduledTaskRepository.GetByIdAsync(id);
            if (scheduledTask == null)
                return null;

            if (scheduledTask.UserId != _userAccessor.UserId)
                throw new ArgumentException("User can only get its own tasks.");

            return scheduledTask;
        }
    }
}
