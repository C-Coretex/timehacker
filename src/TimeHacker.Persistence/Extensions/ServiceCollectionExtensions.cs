using Helpers.Domain.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Persistence.Services.Tasks;

namespace TimeHacker.Persistence.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTimeHackerPersistenceServices(this IServiceCollection services)
        {
            services.AddDataService<IDynamicTasksServiceCommand, DynamicTasksServiceCommand, IDynamicTasksServiceQuery, DynamicTasksServiceQuery, DynamicTasksService>();
            services.AddDataService<IFixedTasksServiceCommand, FixedTasksServiceCommand, IFixedTasksServiceQuery, FixedTasksServiceQuery, FixedTasksService>();

            return services;
        }
    }
}
