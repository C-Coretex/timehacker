using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.IncludeExpansionDelegates;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduleSnapshotService: IScheduleSnapshotService
    {
        private readonly IScheduleSnapshotRepository _scheduleSnapshotRepository;
        private readonly IUserAccessor _userAccessor;

        public ScheduleSnapshotService(IScheduleSnapshotRepository scheduleSnapshotRepository, IUserAccessor userAccessor)
        {
            _scheduleSnapshotRepository = scheduleSnapshotRepository;
            _userAccessor = userAccessor;
        }


        public async Task Add(ScheduleSnapshot scheduleSnapshot)
        {
            var updatedTimestamp = DateTime.UtcNow;

            scheduleSnapshot.UserId = _userAccessor.UserId!;
            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessor.UserId!;
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessor.UserId!;
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            await _scheduleSnapshotRepository.AddAsync(scheduleSnapshot);
        }

        public Task<ScheduleSnapshot?> GetBy(DateOnly date)
        {
            var query = _scheduleSnapshotRepository.GetAll(false, IncludeExpansionScheduleSnapshots.IncludeScheduledTasks, IncludeExpansionScheduleSnapshots.IncludeScheduledCategories);

            return query.FirstOrDefaultAsync(x => x.UserId == _userAccessor.UserId! && x.Date == date);
        }

        public async Task Update(ScheduleSnapshot scheduleSnapshot)
        {
            var updatedTimestamp = DateTime.UtcNow;

            scheduleSnapshot.LastUpdateTimestamp = updatedTimestamp;

            foreach (var scheduledTask in scheduleSnapshot.ScheduledTasks)
            {
                scheduledTask.Date = scheduleSnapshot.Date;
                scheduledTask.UserId = _userAccessor.UserId!;
                scheduledTask.UpdatedTimestamp = updatedTimestamp;
            }
            foreach (var scheduledCategory in scheduleSnapshot.ScheduledCategories)
            {
                scheduledCategory.Date = scheduleSnapshot.Date;
                scheduledCategory.UserId = _userAccessor.UserId!;
                scheduledCategory.UpdatedTimestamp = updatedTimestamp;
            }

            await _scheduleSnapshotRepository.UpdateAsync(scheduleSnapshot);
        }
    }
}
