using AutoMapper;
using TimeHacker.Application.Models.Return.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Application.Profiles.ScheduleSnapshots
{
    public class ScheduleEntityProfile : Profile
    {
        public ScheduleEntityProfile()
        {
            CreateMap<ScheduleEntity, ScheduleEntityReturnModel>();
        }
    }
}
