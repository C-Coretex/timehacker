using AutoMapper;
using TimeHacker.Application.Models.Input.Categories;
using TimeHacker.Application.Models.Return.Categories;
using TimeHacker.Domain.Contracts.Entities.Categories;

namespace TimeHacker.Application.Profiles.Categories
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<InputCategoryModel, Category>();

            CreateMap<Category, CategoryReturnModel>();
        }
    }
}
