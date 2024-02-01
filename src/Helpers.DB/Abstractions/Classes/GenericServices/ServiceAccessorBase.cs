using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Domain.Abstractions.Interfaces;

namespace Helpers.DB.Abstractions.Classes.GenericServices
{
    public class ServiceAccessorBase<TModel> where TModel : class, IModel
    {
        protected DbSet<TModel> dbSet;
        public ServiceAccessorBase(DbSet<TModel> dbSet)
        {
            this.dbSet = dbSet;
        }
    }
}
