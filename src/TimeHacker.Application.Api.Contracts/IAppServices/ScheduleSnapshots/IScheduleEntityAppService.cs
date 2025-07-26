using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots
{
    public interface IScheduleEntityAppService
    {
        Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity);
    }
}
