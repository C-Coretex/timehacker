﻿using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeHacker.Domain.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICategoryService, CategoryService>();

            serviceCollection.AddScoped<IScheduleSnapshotService, ScheduleSnapshotService>();

            serviceCollection.AddScoped<IDynamicTaskService, DynamicTaskService>();
            serviceCollection.AddScoped<IFixedTaskService, FixedTaskService>();
            serviceCollection.AddScoped<ITaskService, TaskService>();

            var mapper = GetMapperConfiguration().CreateMapper();
            serviceCollection.AddSingleton(mapper);

            return serviceCollection;
        }

        static MapperConfiguration GetMapperConfiguration()
        {
            var types = AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(x => x.FullName!.StartsWith("TimeHacker."))
            .SelectMany(s => s.GetTypes())
                                .Where(p => typeof(Profile).IsAssignableFrom(p));

            return new MapperConfiguration(cfg =>
            {
                foreach (var type in types)
                    cfg.AddProfile(type);
            });
        }
    }
}
