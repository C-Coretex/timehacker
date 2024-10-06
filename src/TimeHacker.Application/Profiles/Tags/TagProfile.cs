using AutoMapper;
using TimeHacker.Application.Models.Return.Tags;
using TimeHacker.Domain.Contracts.Entities.Tags;

namespace TimeHacker.Application.Profiles.Tags
{
    public class TagProfile: Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagReturnModel>();
        }
    }
}
