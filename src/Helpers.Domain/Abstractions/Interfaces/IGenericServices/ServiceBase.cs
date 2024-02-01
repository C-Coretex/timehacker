using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Abstractions.Interfaces.IGenericServices
{
    public class ServiceBase<TIServiceCommand, TIServiceQuery, TModel> where TIServiceCommand : IServiceCommandBase<TModel>
                                                                       where TIServiceQuery : IServiceQueryBase<TModel>
                                                                       where TModel : IModel
    {
        public TIServiceCommand Commands { get; init; }
        public TIServiceQuery Queries { get; init; }

        public ServiceBase(TIServiceCommand serviceCommand, TIServiceQuery serviceQuery) 
        { 
            Commands = serviceCommand;
            Queries = serviceQuery;
        }
    }
}
