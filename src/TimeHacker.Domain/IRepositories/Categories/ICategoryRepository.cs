using TimeHacker.Domain.Entities.Categories;

namespace TimeHacker.Domain.IRepositories.Categories;

public interface ICategoryRepository: IUserScopedRepositoryBase<Category, Guid>
{}
