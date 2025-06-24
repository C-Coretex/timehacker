using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduledTaskService: IScheduledTaskService
    {
        private readonly IScheduledTaskRepository _scheduledTaskRepository;

        private readonly UserAccessorBase _userAccessorBase;

        public ScheduledTaskService(IScheduledTaskRepository scheduledTaskRepository, UserAccessorBase userAccessorBase)
        {
            _scheduledTaskRepository = scheduledTaskRepository;
            _userAccessorBase = userAccessorBase;
        }

        public async Task<ScheduledTask?> GetBy(ulong id)
        {
            var scheduledTask = await _scheduledTaskRepository.GetByIdAsync(id);
            if (scheduledTask == null)
                return null;

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (scheduledTask.UserId != _userAccessorBase.UserId)
                throw new ArgumentException("User can only get its own tasks.");

            return scheduledTask;
        }
    }
}
