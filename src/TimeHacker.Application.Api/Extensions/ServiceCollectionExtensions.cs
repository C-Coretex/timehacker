using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Application.Api.Contracts.IServices.Categories;
using TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IServices.Tasks;
using TimeHacker.Application.Api.Contracts.IServices.Users;
using TimeHacker.Application.Api.Services.Categories;
using TimeHacker.Application.Api.Services.ScheduleSnapshots;
using TimeHacker.Application.Api.Services.Tasks;
using TimeHacker.Application.Api.Services.Users;
using TimeHacker.Domain.Services.Extensions;

namespace TimeHacker.Application.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterServices();
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
