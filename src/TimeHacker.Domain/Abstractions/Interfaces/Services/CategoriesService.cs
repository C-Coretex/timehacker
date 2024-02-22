using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Models.Persistence;
using TimeHacker.Domain.Models.Persistence.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services
{
    public interface ICategoriesServiceCommand : IServiceCommandBase<Category>
    {
    }
    public interface ICategoriesServiceQuery : IServiceQueryBase<Category>
    {
    }

    public class CategoriesService : ServiceBase<ICategoriesServiceCommand, ICategoriesServiceQuery, Category>
    {
        public CategoriesService(ICategoriesServiceCommand commands, ICategoriesServiceQuery queries) : base(commands, queries) { }
    }
}
