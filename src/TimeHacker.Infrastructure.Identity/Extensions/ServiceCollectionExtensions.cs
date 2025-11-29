using Microsoft.Extensions.DependencyInjection;

namespace TimeHacker.Infrastructure.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterIdentity(this IServiceCollection services, string identityConnectionString)
        {
            services.AddDbContext<TimeHackerIdentityDbContext>(options =>
                options.UseNpgsql(identityConnectionString));

            return services;
        }
    }
}
