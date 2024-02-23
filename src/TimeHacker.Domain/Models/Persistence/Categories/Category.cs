using Helpers.Domain.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Models.Persistence.Categories
{
    [Index(nameof(UserId))]
    public class Category : IModel
    {
        [Key]
        public int Id { get; init; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(516)]
        public string Description { get; set; }

        [Required]
        public Color Color { get; set; }


        public List<CategoryFixedTask> CategoryFixedTasks { get; set; } = [];

        public List<CategoryDynamicTask> CategoryDynamicTasks { get; set; } = [];
    }
}
