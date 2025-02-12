using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.IncludeExpansionDelegates;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
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

            scheduleSnapshot.UserId = _userAccessorBase.UserId!;
            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.UserId!;
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.UserId!;
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            return _scheduleSnapshotRepository.AddAsync(scheduleSnapshot);
        }

        public Task<ScheduleSnapshot?> GetByAsync(DateOnly date)
        {
            var query = _scheduleSnapshotRepository.GetAll(false, IncludeExpansionScheduleSnapshots.IncludeScheduledTasks, IncludeExpansionScheduleSnapshots.IncludeScheduledCategories);

            return query.FirstOrDefaultAsync(x => x.UserId == _userAccessorBase.UserId! && x.Date == date);
        }

        public Task<ScheduleSnapshot> UpdateAsync(ScheduleSnapshot scheduleSnapshot)
        {
            var updatedTimestamp = DateTime.UtcNow;

            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessorBase.UserId!;
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessorBase.UserId!;
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            return _scheduleSnapshotRepository.UpdateAsync(scheduleSnapshot);
        }
    }
}
