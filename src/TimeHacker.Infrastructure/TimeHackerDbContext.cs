using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.Entities.Users;
using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
using TimeHacker.Infrastructure.Converters;

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
        internal DbSet<ScheduledTask> ScheduledTask { get; set; }
        internal DbSet<ScheduledCategory> ScheduledCategory { get; set; }
        internal DbSet<ScheduleSnapshot> ScheduleSnapshot { get; set; }
        internal DbSet<ScheduleEntity> ScheduleEntity { get; set; }

        //Tasks
        internal DbSet<FixedTask> FixedTask { get; set; }
        internal DbSet<DynamicTask> DynamicTask { get; set; }

        //Tags
        internal DbSet<Tag> Tag { get; set; }
        internal DbSet<TagFixedTask> TagFixedTask { get; set; }
        internal DbSet<TagDynamicTask> TagDynamicTask { get; set; }

        //Users
        internal DbSet<User> User { get; set; }

        #endregion


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Applies all configurations defined in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);

            configurationBuilder
                .Properties<DateTime>()
                .HaveConversion<DateTimeUtcConverter>();
        }
    }
}
