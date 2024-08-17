using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Profiles.ScheduleSnapshots
{
    public class ScheduleSnapshotProfile : Profile
    {
        public ScheduleSnapshotProfile()
        {
            CreateMap<ScheduleSnapshot, TasksForDayReturn>()
                .ForMember(x => x.TasksTimeline, opt => opt.MapFrom(x => x.ScheduledTasks))
                .ForMember(x => x.CategoriesTimeline, opt => opt.MapFrom(x => x.ScheduledCategories));
            CreateMap<TasksForDayReturn, ScheduleSnapshot>()
                .ForMember(x => x.ScheduledTasks, opt => opt.MapFrom(x => x.TasksTimeline))
                .ForMember(x => x.ScheduledCategories, opt => opt.MapFrom(x => x.CategoriesTimeline));
        }
    }
}
