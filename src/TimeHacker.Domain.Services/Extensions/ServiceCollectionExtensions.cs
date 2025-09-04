using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.IProcessors;
using TimeHacker.Domain.IServices;
using TimeHacker.Domain.Services.Processors;
using TimeHacker.Domain.Services.Services;

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
