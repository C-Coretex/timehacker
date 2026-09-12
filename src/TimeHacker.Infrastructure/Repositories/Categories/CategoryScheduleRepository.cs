using TimeHacker.Domain.IRepositories.Categories;

namespace TimeHacker.Infrastructure.Repositories.Categories;

internal sealed class CategoryScheduleRepository(TimeHackerDbContext dbContext, UserAccessorBase userAccessor, TimeProvider timeProvider)
    : UserScopedRepositoryBase<CategorySchedule, Guid>(dbContext ?? throw new ArgumentNullException(nameof(dbContext)), dbContext.CategorySchedule, userAccessor, timeProvider), ICategoryScheduleRepository
{
}
