using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record FixedTaskDto
{
    public Guid? Id { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public byte Priority { get; set; }

    public DateTime StartTimestamp { get; set; }
    public DateTime EndTimestamp { get; set; }

    public DateTime CreatedTimestamp { get; set; }
    public IEnumerable<TagDto> Tags { get; init; } = [];

    public static Expression<Func<FixedTask, FixedTaskDto>> Selector =>
        x => new FixedTaskDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Priority = x.Priority,
            StartTimestamp = x.StartTimestamp,
            EndTimestamp = x.EndTimestamp,
            CreatedTimestamp = x.CreatedTimestamp,
            Tags = x.TagFixedTasks.Select(tagTask => TagDto.Create(tagTask.Tag))
        };

    private static readonly Func<FixedTask, FixedTaskDto> CreateFunc = Selector.Compile();
    public static FixedTaskDto? Create(FixedTask? entity) => entity != null ? CreateFunc(entity) : null;

    public FixedTask GetEntity(FixedTask? entity = null)
    {
        entity ??= new FixedTask();

        entity.Name = Name;
        entity.Description = Description;
        entity.Priority = Priority;
        entity.StartTimestamp = StartTimestamp;
        entity.EndTimestamp = EndTimestamp;

        return entity;
    }
}
