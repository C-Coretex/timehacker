using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TimeHacker.Infrastructure.Converters
{
    internal class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
    {
        public DateTimeUtcConverter()
            : base(
                d => d.ToUniversalTime(),
                d => DateTime.SpecifyKind(d, DateTimeKind.Utc))
        { }
    }
}
