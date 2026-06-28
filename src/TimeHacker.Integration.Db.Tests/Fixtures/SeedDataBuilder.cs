using AutoBogus;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Integration.Db.Tests.Fixtures;

internal sealed class SeedDataBuilder<TRepository, TModel, TId>(TRepository repository) 
    where TRepository : IUserScopedRepositoryBase<TModel, TId>
    where TModel : class, IDbEntity<TId>, IUserScopedEntity
{
    public async Task<TRepository> SeedUsersData(params IEnumerable<UserFixture> otherUsers)
    {
        var model = AutoFaker.Generate<TModel>();
        await repository.AddAndSaveAsync(model, TestContext.Current.CancellationToken);

        foreach (var user in otherUsers)
        {
            var userSeedDataBuilder = user.Resolve<SeedDataBuilder<TRepository, TModel, TId>>();
            await userSeedDataBuilder.SeedUsersData();
        }

        return repository;
    }
}
