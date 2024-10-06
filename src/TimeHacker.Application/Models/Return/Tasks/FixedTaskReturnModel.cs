using TimeHacker.Application.Models.Return.Tags;

namespace TimeHacker.Application.Models.Return.Tasks
{
    public class FixedTaskReturnModel
    {
        public Guid Id { get; init; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte Priority { get; set; }

        public DateTime StartTimestamp { get; set; }
        public DateTime EndTimestamp { get; set; }
        public DateTime CreatedTimestamp { get; set; }

        public IEnumerable<TagReturnModel> Tags { get; set; }
    }
}
