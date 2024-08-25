using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleEntityService
    {
        IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from);
        Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity);
        Task Delete(uint id);
    }
}
