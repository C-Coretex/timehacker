﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Infrastructure;
using TimeHacker.Infrastructure.Repositories.Categories;
using TimeHacker.Infrastructure.Repositories.Tasks;

namespace TimeHacker.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, string timeHackerConnectionString)
        {
            services.AddDbContext<TimeHackerDbContext>(options =>
                options.UseSqlServer(timeHackerConnectionString));

            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IFixedTaskRepository, FixedTaskRepository>();
            services.AddScoped<IDynamicTaskRepository, DynamicTaskRepository>();

            return services;
        }
    }
}
