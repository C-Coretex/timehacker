using Helpers.DB.Abstractions.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeHacker.Domain.Models.Tasks;

namespace TimeHacker.Persistence.Context
{
    public class TimeHackerDBContext: DbContextBase<TimeHackerDBContext>
    {
        public TimeHackerDBContext(DbContextOptions<TimeHackerDBContext> options) : base(options) { }
        public TimeHackerDBContext(string connectionString) : base(connectionString) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        internal DbSet<FixedTask> FixedTasks { get; set; }
        internal DbSet<DynamicTask> DynamicTasks { get; set; }
    }
}
