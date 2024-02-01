using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Domain.Abstractions.Interfaces
{
    public interface IContextFactory<TContext> where TContext : class
    {
        public TContext CreateContext();
    }
}
