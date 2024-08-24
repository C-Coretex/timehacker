using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Infrastructure;
using TimeHacker.Infrastructure.Repositories.Categories;
using TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;
using TimeHacker.Infrastructure.Repositories.Tasks;

namespace TimeHacker.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, string timeHackerConnectionString)
        {
            services.AddDbContext<TimeHackerDbContext>(options =>
                options.UseSqlServer(timeHackerConnectionString));

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IScheduleSnapshotRepository, ScheduleSnapshotRepository>();
            services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
            services.AddScoped<IScheduledCategoryRepository, ScheduledCategoryRepository>();
            services.AddScoped<IScheduleEntityRepository, ScheduleEntityRepository>();

            services.AddScoped<IFixedTaskRepository, FixedTaskRepository>();
            services.AddScoped<IDynamicTaskRepository, DynamicTaskRepository>();

            return services;
        }
    }
}
