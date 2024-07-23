using Helpers.DB.Abstractions.Classes;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Infrastructure
{
    public class TimeHackerDbContext : DbContextBase<TimeHackerDbContext>
    {
        public TimeHackerDbContext(DbContextOptions<TimeHackerDbContext> options) : base(options) { }
        public TimeHackerDbContext(string connectionString) : base(connectionString) { }

        internal DbSet<FixedTask> FixedTask { get; set; }
        internal DbSet<DynamicTask> DynamicTask { get; set; }
        internal DbSet<Category> Category { get; set; }
        internal DbSet<CategoryFixedTask> CategoryFixedTask { get; set; }
        internal DbSet<CategoryDynamicTask> CategoryDynamicTask { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies all configurations defined in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
