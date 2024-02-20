using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Abstractions.Interfaces.IGenericServices
{
    public interface IServiceQueryBase<TModel> where TModel : IModel
    {
        public IQueryable<TModel> GetAll();
        public TModel? GetById(int id);
        public Task<TModel?> GetByIdAsync(int id);
    }
}
