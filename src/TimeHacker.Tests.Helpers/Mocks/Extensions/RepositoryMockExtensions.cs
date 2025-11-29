namespace TimeHacker.Helpers.Tests.Mocks.Extensions
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

            repository.Setup(x => x.GetByIdAsync(It.IsAny<TId>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns<TId, bool, CancellationToken, QueryPipelineStep<TModel>[]>((id, _, _, _) => Task.FromResult(source.FirstOrDefault(x => x.Id!.Equals(id))));

            repository.Setup(x => x.DeleteAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.RemoveAll(x => x.Id!.Equals(entry.Id)));
        }

        public static void SetupRepositoryMock<TModel>(this Mock<IRepositoryBase<TModel>> repository, List<TModel> source) where TModel : class, IDbEntity
        {
            repository.Setup(x => x.AddAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.Add(entry))
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetAll(It.IsAny<bool>()))
            .Returns(source.BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns(source.BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns(source.BuildMock());
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

            repository.Setup(x => x.Update(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) =>
                {
                    source.RemoveAll(x => x.Id!.Equals(entry.Id));
                    source.Add(entry);
                })
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));

            repository.Setup(x => x.GetByIdAsync(It.IsAny<TId>(), It.IsAny<bool>(), It.IsAny<CancellationToken>(), It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns<TId, bool, CancellationToken, QueryPipelineStep<TModel>[]>((id, _, _, _) => Task.FromResult(source.FirstOrDefault(x => x.Id!.Equals(id))));

            repository.Setup(x => x.ExistsAsync(It.IsAny<TId>(),It.IsAny<CancellationToken>()))
                .Returns<TId, CancellationToken>((id, _) => Task.FromResult(source.Any(x => x.Id!.Equals(id))));

            repository.Setup(x => x.DeleteAndSaveAsync(It.IsAny<TId>(), It.IsAny<CancellationToken>()))
                .Callback<TId, CancellationToken>((id, _) => source.RemoveAll(x => x.Id!.Equals(id)));

            repository.Setup(x => x.AddAndSaveAsync(It.IsAny<TModel>(), It.IsAny<CancellationToken>()))
                .Callback<TModel, CancellationToken>((entry, _) => source.Add(entry))
                .Returns<TModel, CancellationToken>((entry, _) => Task.FromResult(entry));
            repository.Setup(x => x.Add(It.IsAny<TModel>()))
                .Callback<TModel>(source.Add)
                .Returns<TModel>((entry) => entry);

            repository.Setup(x => x.GetAll(It.IsAny<bool>()))
                .Returns(source.BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns(source.BuildMock());

            repository.Setup(x => x.GetAll(It.IsAny<bool>(), It.IsAny<QueryPipelineStep<TModel>[]>()))
                .Returns(source.BuildMock());
        }

    }
}
