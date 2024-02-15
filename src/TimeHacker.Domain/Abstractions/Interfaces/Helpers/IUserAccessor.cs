using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeHacker.Domain.Abstractions.Interfaces.Helpers
{
    public interface IUserAccessor
    {
        public string UserId { get; }

        public bool IsUserValid { get; }
    }
}
