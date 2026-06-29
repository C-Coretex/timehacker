using AutoBogus;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

/// <summary>
/// Generic single-entity seeder: persists one AutoFaker-generated row of any user-scoped entity through
/// its repository (so UserId is stamped automatically).
/// </summary>
internal sealed class SeedDataBuilder<TRepository, TModel, TId>(TRepository repository)
    where TRepository : IUserScopedRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
    public Task<TModel> SeedForCurrentUser()
        => repository.AddAndSaveAsync(AutoFaker.Generate<TModel>(), TestContext.Current.CancellationToken);

    public async Task<TRepository> SeedUsersData(params IEnumerable<UserFixture> otherUsers)
    {
        await SeedForCurrentUser();

        foreach (var user in otherUsers)
            await user.Resolve<SeedDataBuilder<TRepository, TModel, TId>>().SeedForCurrentUser();

        return repository;
    }
}
