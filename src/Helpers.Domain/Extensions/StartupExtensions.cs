using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Domain.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDataService<TICommand, TCommand, TIQuery, TQuery, TService>(this IServiceCollection services) 
                                                                                                                           where TCommand : class, TICommand
                                                                                                                           where TICommand : class
                                                                                                                           where TQuery : class, TIQuery
                                                                                                                           where TIQuery : class
                                                                                                                           where TService : class
        {
            services.AddScoped<TICommand, TCommand>();
            services.AddScoped<TIQuery, TQuery>();
            services.AddScoped<TService>();

            return services;
        }

        public static IServiceCollection AddDataService<TCommand, TQuery, TService>(this IServiceCollection services)
                                                                                                                   where TCommand : class
                                                                                                                   where TQuery : class
                                                                                                                   where TService : class
        {
            services.AddScoped<TCommand>();
            services.AddScoped<TQuery>();
            services.AddScoped<TService>();

            return services;
        }
    }
}
