using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Models.ReturnModels;

namespace TimeHacker.Domain.IServices
{
    public interface IScheduleEntityService
    {
        IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from);

        Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated);
    }
}
