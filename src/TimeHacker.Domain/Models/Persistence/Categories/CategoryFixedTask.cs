using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Models.Persistence.Categories
{
    [PrimaryKey(nameof(CategoryId), nameof(FixedTaskId))]
    public class CategoryFixedTask
    {
        public int CategoryId { get; set; }
        public int FixedTaskId { get; set; }

        [NotMapped]
        public Category Category { get; set; } = null!;
        [NotMapped]
        public FixedTask FixedTask { get; set; } = null!;
    }
}
