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
            scheduleSnapshot.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
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
            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            }

            return _scheduleSnapshotRepository.UpdateAndSaveAsync(scheduleSnapshot);
        }
    }
}
