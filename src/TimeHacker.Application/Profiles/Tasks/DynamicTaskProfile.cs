using AutoMapper;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Application.Models.Return.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Application.Profiles.Tasks
{
    public class DynamicTaskProfile: Profile
    {
        public DynamicTaskProfile()
        {
            CreateMap<InputDynamicTaskModel, DynamicTask>();

            CreateMap<DynamicTask, DynamicTaskReturnModel>()
                .ForMember(x => x.Tags, opt => opt.MapFrom(x => x.TagDynamicTasks.Select(y => y.Tag)));
        }
    }
}
