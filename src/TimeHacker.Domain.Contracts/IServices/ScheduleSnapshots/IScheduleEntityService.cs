using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots
{
    public interface IScheduleEntityService
    {
        IQueryable<ScheduleEntity> GetAll(bool onlyActive = true);
        Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity);
        Task Delete(uint id);
    }
}
