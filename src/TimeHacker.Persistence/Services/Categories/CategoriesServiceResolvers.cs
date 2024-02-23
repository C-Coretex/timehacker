using Helpers.DB.Abstractions.Classes.GenericServices;
using TimeHacker.Domain.Abstractions.Interfaces.Services.Categories;
using TimeHacker.Domain.Models.Persistence.Categories;
using TimeHacker.Persistence.Context;

namespace TimeHacker.Persistence.Services.Categories
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
