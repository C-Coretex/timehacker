using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;
using TimeHacker.Domain.Contracts.Models.ReturnModels;

namespace TimeHacker.Domain.Profiles.ScheduleSnapshots
{
    public class ScheduleTaskProfile : Profile
    {
        public ScheduleTaskProfile()
        {
            CreateMap<ScheduledTask, TaskContainerReturn>()
                .ForMember(x => x.TimeRange, opt => opt.MapFrom(x => new TimeRange(x.Start, x.End)))
                .ForMember(x => x.IsFixed, opt => opt.MapFrom(x => x.IsFixed))
                .ForMember(x => x.Task, opt => opt.MapFrom(x => x.IsFixed ? (ITask)new FixedTask() : new DynamicTask()))
                .ForMember(x => x.ScheduleEntityId, opt => opt.MapFrom(x => x.ParentScheduleEntityId))
                .ForPath(x => x.Task.UserId, opt => opt.MapFrom(x => x.UserId))
                .ForPath(x => x.Task.Name, opt => opt.MapFrom(x => x.Name))
                .ForPath(x => x.Task.Description, opt => opt.MapFrom(x => x.Description))
                .ForPath(x => x.Task.Priority, opt => opt.MapFrom(x => x.Priority))
                .ForPath(x => x.Task.Id, opt => opt.MapFrom(x => x.ParentTaskId));

            CreateMap<TaskContainerReturn, ScheduledTask>()
                .ForMember(x => x.Start, opt => opt.MapFrom(x => x.TimeRange.Start))
                .ForMember(x => x.End, opt => opt.MapFrom(x => x.TimeRange.End))
                .ForMember(x => x.IsFixed, opt => opt.MapFrom(x => x.IsFixed))
                .ForMember(x => x.UserId, opt => opt.MapFrom(x => x.Task.UserId))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Task.Name))
                .ForMember(x => x.Description, opt => opt.MapFrom(x => x.Task.Description))
                .ForMember(x => x.Priority, opt => opt.MapFrom(x => x.Task.Priority))
                .ForMember(x => x.ParentTaskId, opt => opt.MapFrom(x => x.Task.Id))
                .ForMember(x => x.ParentScheduleEntityId, opt => opt.MapFrom(x => x.ScheduleEntityId));
        }
    }
}
