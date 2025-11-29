using Microsoft.Extensions.DependencyInjection;

namespace TimeHacker.Domain.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDomainServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IScheduleEntityService, ScheduleEntityService>();

            serviceCollection.AddScoped<ITaskTimelineProcessor, TaskTimelineProcessor>();

            return serviceCollection;
        }
    }
}
