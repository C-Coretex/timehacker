using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;

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
            scheduleSnapshot.UserId = _userAccessor.UserId!;
            scheduleSnapshot.LastUpdateTimestamp = DateTime.UtcNow;
            await _scheduleSnapshotRepository.AddAsync(scheduleSnapshot);
        }

        public Task<ScheduleSnapshot?> GetBy(DateOnly date)
        {
            return _scheduleSnapshotRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == _userAccessor.UserId! && x.Date == date);
        }

        public async Task Update(ScheduleSnapshot scheduleSnapshot)
        {
            scheduleSnapshot.LastUpdateTimestamp = DateTime.UtcNow;
            await _scheduleSnapshotRepository.UpdateAsync(scheduleSnapshot);
        }
    }
}
