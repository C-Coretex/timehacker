using Helpers.DB.Abstractions.Classes;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;

namespace TimeHacker.Infrastructure
{
    public class TimeHackerDbContext : DbContextBase<TimeHackerDbContext>
    {
        public TimeHackerDbContext(DbContextOptions<TimeHackerDbContext> options) : base(options) { }
        public TimeHackerDbContext(string connectionString) : base(connectionString) { }

        #region DbSets

        //Categories
        internal DbSet<Category> Category { get; set; }
        internal DbSet<CategoryFixedTask> CategoryFixedTask { get; set; }
        internal DbSet<CategoryDynamicTask> CategoryDynamicTask { get; set; }

        //ScheduleSnapshots
        internal DbSet<ScheduleSnapshot> ScheduleSnapshot { get; set; }

        //Tasks
        internal DbSet<FixedTask> FixedTask { get; set; }
        internal DbSet<DynamicTask> DynamicTask { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applies all configurations defined in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
