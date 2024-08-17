using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Profiles.ScheduleSnapshots
{
    public class ScheduleCategoryProfile : Profile
    {
        public ScheduleCategoryProfile()
        {
            CreateMap<ScheduledCategory, CategoryContainerReturn>()
                .ForMember(x => x.TimeRange, opt => opt.MapFrom(x => new TimeRange(x.Start, x.End)));
            CreateMap<CategoryContainerReturn, ScheduledCategory>()
                .ForMember(x => x.Start, opt => opt.MapFrom(x => x.TimeRange.Start))
                .ForMember(x => x.End, opt => opt.MapFrom(x => x.TimeRange.End));
        }
    }
}
