using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tags;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Users;
using TimeHacker.Infrastructure.Repositories.Categories;
using TimeHacker.Infrastructure.Repositories.ScheduleSnapshots;
using TimeHacker.Infrastructure.Repositories.Tags;
using TimeHacker.Infrastructure.Repositories.Tasks;
using TimeHacker.Infrastructure.Repositories.Users;

namespace TimeHacker.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, string timeHackerConnectionString)
        {
            services.AddDbContext<TimeHackerDbContext>(options =>
                options.UseNpgsql(timeHackerConnectionString));

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IScheduleSnapshotRepository, ScheduleSnapshotRepository>();
            services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
            services.AddScoped<IScheduledCategoryRepository, ScheduledCategoryRepository>();
            services.AddScoped<IScheduleEntityRepository, ScheduleEntityRepository>();

            services.AddScoped<IFixedTaskRepository, FixedTaskRepository>();
            services.AddScoped<IDynamicTaskRepository, DynamicTaskRepository>();

            services.AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
