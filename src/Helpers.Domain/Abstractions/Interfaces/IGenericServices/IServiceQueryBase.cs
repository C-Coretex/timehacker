namespace Helpers.Domain.Abstractions.Interfaces.IGenericServices
{
    public interface IServiceQueryBase<TModel> where TModel : IModel
    {
        public IQueryable<TModel> GetAll();
        public TModel? GetById(int id);
        public Task<TModel?> GetByIdAsync(int id);
    }
}
