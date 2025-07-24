using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.IncludeExpansionDelegates;

namespace TimeHacker.Domain.Services.Services.ScheduleSnapshots
{
    public class ScheduleSnapshotService(IScheduleSnapshotRepository scheduleSnapshotRepository, UserAccessorBase userAccessorBase)
        : IScheduleSnapshotService
    {
        public Task<ScheduleSnapshot> AddAsync(ScheduleSnapshot scheduleSnapshot)
        {
            var userId = userAccessorBase.GetUserIdOrThrowUnauthorized();
            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = userId;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = userId;
            }

            return scheduleSnapshotRepository.AddAndSaveAsync(scheduleSnapshot);
        }

        public Task<ScheduleSnapshot?> GetByAsync(DateOnly date)
        {
            var query = scheduleSnapshotRepository.GetAll(false, IncludeExpansionScheduleSnapshots.IncludeScheduledTasks, IncludeExpansionScheduleSnapshots.IncludeScheduledCategories);
            return query.FirstOrDefaultAsync(x => x.Date == date);
        }

        public Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot)
        {
            var userId = userAccessorBase.GetUserIdOrThrowUnauthorized();
            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = userId;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = userId;
            }

            return scheduleSnapshotRepository.UpdateAndSaveAsync(scheduleSnapshot);
        }
    }
}
