using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.Users;
using TimeHacker.Domain.Contracts.Models.InputModels.Users;

namespace TimeHacker.Domain.Profiles.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserUpdateModel>();
            CreateMap<UserUpdateModel, User>();
        }
    }
}
