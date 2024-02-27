using Helpers.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Categories;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks.UserTasks;
using TimeHacker.Persistence.Services.Categories;
using TimeHacker.Persistence.Services.Tasks;
using TimeHacker.Persistence.Services.Tasks.UserTasks;

namespace TimeHacker.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTimeHackerPersistenceServices(this IServiceCollection services)
        {
            services.AddDataService<IDynamicTasksServiceCommand, DynamicTasksServiceCommand, IDynamicTasksServiceQuery, DynamicTasksServiceQuery, DynamicTasksService>();
            services.AddDataService<IFixedTasksServiceCommand, FixedTasksServiceCommand, IFixedTasksServiceQuery, FixedTasksServiceQuery, FixedTasksService>();

            services.AddDataService<IDynamicUserTasksServiceCommand, DynamicUserTasksServiceCommand, IDynamicUserTasksServiceQuery, DynamicUserTasksServiceQuery, DynamicUserTasksService>();
            services.AddDataService<IFixedUserTasksServiceCommand, FixedUserTasksServiceCommand, IFixedUserTasksServiceQuery, FixedUserTasksServiceQuery, FixedUserTasksService>();

            services.AddDataService<ICategoriesServiceCommand, CategoriesServiceCommand, ICategoriesServiceQuery, CategoriesServiceQuery, CategoriesService>();

            return services;
        }
    }
}
