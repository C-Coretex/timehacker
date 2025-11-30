using System.Drawing;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TimeHacker.Infrastructure.Converters;

internal class ColorConverter : ValueConverter<Color, int>
{
    public ColorConverter()
        : base(
            v => v.ToArgb(),
            v => Color.FromArgb(v))
    { }
}
