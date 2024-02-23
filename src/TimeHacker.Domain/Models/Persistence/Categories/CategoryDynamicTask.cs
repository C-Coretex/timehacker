using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Models.Persistence.Categories
{
    [PrimaryKey(nameof(CategoryId), nameof(DynamicTaskId))]
    public class CategoryDynamicTask
    {
        public int CategoryId { get; set; }
        public int DynamicTaskId { get; set; }

        public Category Category { get; set; } = null!;
        public DynamicTask DynamicTask { get; set; } = null!;
    }
}
