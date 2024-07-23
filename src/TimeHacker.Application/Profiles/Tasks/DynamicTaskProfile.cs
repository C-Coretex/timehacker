using AutoMapper;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Application.Profiles.Tasks
{
    public class DynamicTaskProfile: Profile
    {
        public DynamicTaskProfile()
        {
            CreateMap<InputDynamicTaskModel, DynamicTask>();
        }
    }
}
