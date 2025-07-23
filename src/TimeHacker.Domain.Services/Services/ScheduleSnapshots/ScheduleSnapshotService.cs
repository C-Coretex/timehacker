using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.IncludeExpansionDelegates;

namespace TimeHacker.Domain.Services.Services.ScheduleSnapshots
{
    public class ScheduleSnapshotService: IScheduleSnapshotService
    {
        private readonly IScheduleSnapshotRepository _scheduleSnapshotRepository;
        private readonly UserAccessorBase _userAccessorBase;

        public ScheduleSnapshotService(IScheduleSnapshotRepository scheduleSnapshotRepository, UserAccessorBase userAccessorBase)
        {
            _scheduleSnapshotRepository = scheduleSnapshotRepository;

            _userAccessorBase = userAccessorBase;
        }


        public Task<ScheduleSnapshot> AddAsync(ScheduleSnapshot scheduleSnapshot)
        {
            var updatedTimestamp = DateTime.UtcNow;

            scheduleSnapshot.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            return _scheduleSnapshotRepository.AddAndSaveAsync(scheduleSnapshot);
        }

        public Task<ScheduleSnapshot?> GetByAsync(DateOnly date)
        {
            var query = _scheduleSnapshotRepository.GetAll(false, IncludeExpansionScheduleSnapshots.IncludeScheduledTasks, IncludeExpansionScheduleSnapshots.IncludeScheduledCategories);

            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            return query.FirstOrDefaultAsync(x => x.UserId == userId && x.Date == date);
        }

        public Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot)
        {
            var updatedTimestamp = DateTime.UtcNow;

            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            return _scheduleSnapshotRepository.UpdateAndSaveAsync(scheduleSnapshot);
        }
    }
}
