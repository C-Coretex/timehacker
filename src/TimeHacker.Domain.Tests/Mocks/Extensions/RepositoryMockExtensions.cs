using MockQueryable.Moq;
using Moq;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Tests.Mocks.Extensions
{
    public static class RepositoryMockExtensions
    {
        public static void SetupRepositoryMock<TModel, TId>(this Mock<IRepositoryBase<TModel, TId>> repository, List<TModel> source) where TModel : class, IDbModel<TId>, new()
        {
            repository.Setup(x => x.AddAsync(It.IsAny<TModel>(), It.IsAny<bool>()))
                .Callback<TModel, bool>((entry, _) => source.Add(entry));

            repository.Setup(x => x.UpdateAsync(It.IsAny<TModel>(), It.IsAny<bool>()))
                .Callback<TModel, bool>((entry, _) =>
                {
                    source.RemoveAll(x => x.Id!.Equals(entry.Id));
                    source.Add(entry);
                })
                .Returns<TModel, bool>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetByIdAsync(It.IsAny<TId>(), It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns<TId, bool, IncludeExpansionDelegate<TModel>[]>((id, _, _) => Task.FromResult(source.FirstOrDefault(x => x.Id!.Equals(id))));

            repository.Setup(x => x.DeleteAsync(It.IsAny<TModel>(), It.IsAny<bool>()))
                .Callback<TModel, bool>((entry, _) => source.RemoveAll(x => x.Id!.Equals(entry.Id)));

            repository.Setup(x => x.GetAll(It.IsAny<bool>()))
            .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<TModel>[]>()))
                .Returns(source.AsQueryable().BuildMock());
        }
    }
}
