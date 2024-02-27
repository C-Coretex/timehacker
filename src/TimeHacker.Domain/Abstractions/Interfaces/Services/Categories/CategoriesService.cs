using Helpers.Domain.Abstractions.Interfaces.IGenericServices;
using TimeHacker.Domain.Models.Persistence.Categories;

namespace TimeHacker.Domain.Abstractions.Interfaces.Services.Categories
{
    public interface ICategoriesServiceCommand : IServiceCommandBase<Category>
    {
    }
    public interface ICategoriesServiceQuery : IServiceQueryBase<Category>
    {
        public IQueryable<Category> GetCategoriesForUserId(string userId);
    }

    public class CategoriesService : ServiceBase<ICategoriesServiceCommand, ICategoriesServiceQuery, Category>
    {
        public CategoriesService(ICategoriesServiceCommand commands, ICategoriesServiceQuery queries) : base(commands, queries) { }
    }
}
