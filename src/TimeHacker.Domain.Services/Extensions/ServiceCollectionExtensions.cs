using Microsoft.Extensions.DependencyInjection;

namespace TimeHacker.Domain.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}
