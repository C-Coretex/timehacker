using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.IRepositories.Categories
{
    public interface ICategoryRepository: IRepositoryBase<Category, Guid>
    {}
}
