using AutoMapper;
using TimeHacker.Application.Models.Input.Categories;
using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Application.Profiles.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<InputCategoryModel, Category>();
        }
    }
}
