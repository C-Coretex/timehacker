using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeHacker.Domain.Constants
{
    public static class DefaultConstants
    {
        public static readonly TimeSpan StartOfDay = new(0, 0, 0);
        public static readonly TimeSpan EndOfDay = new(24, 0, 0);
        public static readonly TimeSpan TimeBacklashBetweenTasks = new(0, 5, 0);
    }
}
