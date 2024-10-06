using System.Drawing;

namespace TimeHacker.Application.Models.Return.Tags
{
    public class TagReturnModel
    {
        public Guid Id { get; init; }

        public string Name { get; set; }
        public string? Category { get; set; }

        public Color Color { get; set; }
    }
}
