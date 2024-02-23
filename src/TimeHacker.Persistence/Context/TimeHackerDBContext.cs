using Helpers.DB.Abstractions.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using TimeHacker.Domain.Models.Persistence.Categories;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Persistence.Context
{
    public class TimeHackerDBContext : DbContextBase<TimeHackerDBContext>
    {
        public TimeHackerDBContext(DbContextOptions<TimeHackerDBContext> options) : base(options) { }
        public TimeHackerDBContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(c => c.Color)
                .HasConversion(
                    v => v.ToArgb(), 
                    v => Color.FromArgb(v)
            );
        }

        internal DbSet<FixedTask> FixedTasks { get; set; }
        internal DbSet<DynamicTask> DynamicTasks { get; set; }
        internal DbSet<Category> Categories { get; set; }
        internal DbSet<CategoryFixedTask> CategoryFixedTasks { get; set; }
        internal DbSet<CategoryDynamicTask> CategoryDynamicTasks { get; set; }
    }
}
