using System.Drawing;
using System.Linq.Expressions;
using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Application.Api.Contracts.DTOs.Categories
{
    public record CategoryDto(
        Guid? Id,
        Guid? ScheduleEntityId,
        string Name,
        string? Description,
        Color Color)
    {
        public CategoryDto(
            Guid? ScheduleEntityId,
            string Name,
            string? Description,
            Color Color) : this(Guid.Empty, ScheduleEntityId, Name, Description, Color)
        {
        }

        public Category GetEntity(Category? category = null)
        {
            category ??= new Category()
            {
                Id = Id ?? Guid.CreateVersion7()
            };

            category.ScheduleEntityId = ScheduleEntityId;
            category.Name = Name;
            category.Description = Description;
            category.Color = Color;

            return category;
        }


        public static Expression<Func<Category, CategoryDto>> Selector =>
            category => new CategoryDto(category.Id, category.ScheduleEntityId, category.Name, category.Description, category.Color);

        private static readonly Func<Category, CategoryDto> CreateFunc = Selector.Compile();
        public static CategoryDto Create(Category category) => CreateFunc(category);
    }
}
