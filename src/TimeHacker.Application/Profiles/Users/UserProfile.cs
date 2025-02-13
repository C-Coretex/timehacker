using AutoMapper;
using TimeHacker.Application.Models.Return.Users;
using TimeHacker.Domain.Contracts.Entities.Users;

namespace TimeHacker.Application.Profiles.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReturnModel>();
        }
    }
}
