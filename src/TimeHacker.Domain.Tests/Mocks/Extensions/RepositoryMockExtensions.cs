using MockQueryable;
using Moq;
using TimeHacker.Domain.Entities.EntityBase;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Domain.Tests.Mocks.Extensions
{
    public static class RepositoryMockExtensions
    {
        public static void SetupRepositoryMock<TModel, TId>(this Mock<IRepositoryBase<TModel, TId>> repository, List<TModel> source) where TModel : class, IDbEntity<TId>
        {
            repository.As<IRepositoryBase<TModel>>().SetupRepositoryMock(source);

            repository.Setup(x => x.UpdateAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) =>
                {
                    source.RemoveAll(x => x.Id!.Equals(entry.Id));
                    source.Add(entry);
                })
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetByIdAsync(It.IsAny<TId>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns<TId, bool, CancellationToken, IncludeExpansionDelegate<TModel>[]>((id, _, _, _) => Task.FromResult(source.FirstOrDefault(x => x.Id!.Equals(id))));

            repository.Setup(x => x.DeleteAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.RemoveAll(x => x.Id!.Equals(entry.Id)));
        }

        public static void SetupRepositoryMock<TModel>(this Mock<IRepositoryBase<TModel>> repository, List<TModel> source) where TModel : class, IDbEntity
        {
            repository.Setup(x => x.AddAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.Add(entry))
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetAll(It.IsAny<bool>()))
            .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());
        }

        public static void SetupRepositoryMock<TModel, TId>(this Mock<IUserScopedRepositoryBase<TModel, TId>> repository, List<TModel> source) where TModel : class, IUserScopedEntity, IDbEntity<TId>
        {
            repository.As<IRepositoryBase<TModel>>().SetupRepositoryMock(source);

            repository.Setup(x => x.UpdateAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) =>
                {
                    source.RemoveAll(x => x.Id!.Equals(entry.Id));
                    source.Add(entry);
                })
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetByIdAsync(It.IsAny<TId>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns<TId, bool, CancellationToken, IncludeExpansionDelegate<TModel>[]>((id, _, _, _) => Task.FromResult(source.FirstOrDefault(x => x.Id!.Equals(id))));

            repository.Setup(x => x.DeleteAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.RemoveAll(x => x.Id!.Equals(entry.Id)));

            repository.Setup(x => x.AddAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.Add(entry))
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetAll(It.IsAny<bool>()))
                .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());
        }

    }
}
