using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Profiles.ScheduleSnapshots
{
    public class ScheduleEntityProfile : Profile
    {
        public ScheduleEntityProfile()
        {
            CreateMap<ScheduleEntity, ScheduleEntityReturn>();
            CreateMap<ScheduleEntityReturn, ScheduleEntity>();
        }
    }
}
