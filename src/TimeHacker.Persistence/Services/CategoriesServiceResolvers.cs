using Helpers.DB.Abstractions.Classes.GenericServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Abstractions.Interfaces.Services;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Tasks;
using TimeHacker.Domain.Models.Persistence;
using TimeHacker.Domain.Models.Persistence.Tasks;
using TimeHacker.Persistence.Context;

namespace TimeHacker.Persistence.Services
{
    public class CategoriesServiceCommand : ServiceCommandBase<TimeHackerDBContext, Category>, ICategoriesServiceCommand
    {
        public CategoriesServiceCommand(TimeHackerDBContext dbContext) : base(dbContext, dbContext.Categories) { }
    }

    public class CategoriesServiceQuery : ServiceQueryBase<Category>, ICategoriesServiceQuery
    {
        public CategoriesServiceQuery(TimeHackerDBContext dbContext) : base(dbContext.Categories) { }
    }
}
