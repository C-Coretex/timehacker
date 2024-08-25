using AutoMapper;
using TimeHacker.Application.Models.Input.Tasks;
using TimeHacker.Application.Models.Return.Tasks;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Application.Profiles.Tasks
{
    public class FixedTaskProfile : Profile
    {
        public FixedTaskProfile()
        {
            CreateMap<InputFixedTaskModel, FixedTask>();

            CreateMap<FixedTask, FixedTaskReturnModel>();
        }
    }
}
