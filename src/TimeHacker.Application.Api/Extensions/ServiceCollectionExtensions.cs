using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Application.Api.AppServices.Categories;
using TimeHacker.Application.Api.AppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.AppServices.Tasks;
using TimeHacker.Application.Api.AppServices.Users;
using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
using TimeHacker.Application.Api.Contracts.IAppServices.Users;

namespace TimeHacker.Application.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAppServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ICategoryAppService, CategoryService>();

        serviceCollection.AddScoped<IScheduledTaskAppService, ScheduledTaskAppService>();
        serviceCollection.AddScoped<IScheduleEntityAppService, ScheduleEntityAppService>();
        serviceCollection.AddScoped<IScheduledCategoryAppService, ScheduledCategoryService>();

        serviceCollection.AddScoped<IDynamicTaskAppService, DynamicTaskAppService>();
        serviceCollection.AddScoped<IFixedTaskAppService, FixedTaskAppService>();
        serviceCollection.AddScoped<ITaskAppService, TaskService>();

        serviceCollection.AddScoped<ITaskAppService, TaskService>();

        serviceCollection.AddScoped<IUserAppService, UserService>();

        return serviceCollection;
    }
}
