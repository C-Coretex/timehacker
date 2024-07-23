using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Domain.Services.Tasks;

namespace TimeHacker.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<Contracts.IServices.Categories.ICategoryService, CategoryService>();

            serviceCollection.AddScoped<IDynamicTaskService, DynamicTaskService>();
            serviceCollection.AddScoped<Contracts.IServices.Tasks.IFixedTaskService, FixedTaskService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();

            return serviceCollection;
        }
    }
}
