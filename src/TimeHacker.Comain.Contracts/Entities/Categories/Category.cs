using System.Drawing;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Contracts.Entities.Categories
{
    public class Category : IDbModel<int>
    {
        public int Id { get; init; }

        public string? UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public Color Color { get; set; }


        public List<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];

        public List<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
