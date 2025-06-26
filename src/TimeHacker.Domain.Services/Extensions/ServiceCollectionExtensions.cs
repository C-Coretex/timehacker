using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.IServices.Categories;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.IServices.Tasks;
using TimeHacker.Domain.IServices.Users;
using TimeHacker.Domain.Services.Services.Categories;
using TimeHacker.Domain.Services.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Services.Tasks;
using TimeHacker.Domain.Services.Services.Users;

namespace TimeHacker.Domain.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICategoryService, CategoryService>();

            serviceCollection.AddScoped<IScheduleSnapshotService, ScheduleSnapshotService>();
            serviceCollection.AddScoped<IScheduledTaskService, ScheduledTaskService>();
            serviceCollection.AddScoped<IScheduledCategoryService, ScheduledCategoryService>();
            serviceCollection.AddScoped<IScheduleEntityService, ScheduleEntityService>();

            serviceCollection.AddScoped<IDynamicTaskService, DynamicTaskService>();
            serviceCollection.AddScoped<IFixedTaskService, FixedTaskService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();

            serviceCollection.AddScoped<ITaskService, TaskService>();

            serviceCollection.AddScoped<IUserService, UserService>();

            return serviceCollection;
        }
    }
}
