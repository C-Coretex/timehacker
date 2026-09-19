using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Helpers.Domain.Extensions;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record FixedTaskDto
{
    public Guid? Id { get; init; }

    public required string Name { get; init; }
    public string? Description { get; init; }
    public byte Priority { get; init; }

    public DateTime StartTimestamp { get; init; }
    public DateTime EndTimestamp { get; init; }

    public DateTime CreatedTimestamp { get; init; }
    public ICollection<LinkCategoryDto> Categories { get; init => field = [.. value.DistinctBy(c => c.Id)]; } = [];
    public IEnumerable<CategoryDto> RichCategories => Categories.OfType<CategoryDto>();
    public IEnumerable<TagDto> Tags { get; init; } = [];
    public ScheduleEntityDto? ScheduleEntity { get; init; }

    // actually selector should be the last step of the query pipeline
    // it's not safe to query on a DTO that has nested classes inside it.
    // also, the DTO is a business domain representation, not a database representation
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
            // AsQueryable keeps CategoryDto.Selector visible to the translator, so each category's Schedules
            // are projected too; CategoryDto.Create compiles just as well but its opaque Func hides them from
            // EF and they come back silently empty. ToList: EF rejects an IQueryable in a final projection.
            //TODO: check if it's really needed (it's needed to preserve ScheduledEntitties - navigation property of the Category). If it's really needed we need to implement it everywhere
            Categories = x.CategoryFixedTasks.AsQueryable().Select(catTask => catTask.Category).Select(CategoryDto.Selector)
                .OfType<LinkCategoryDto>().ToList(),
            Tags = x.TagFixedTasks.Select(tagTask => TagDto.Create(tagTask.Tag)),
            ScheduleEntity = x.ScheduleEntity != null ? ScheduleEntityDto.Create(x.ScheduleEntity) : null
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

    /// <summary>
    /// <see cref="GetEntity"/> plus the category junctions, replacing whatever the entity currently
    /// carries. The entity must be loaded with its junctions or EF cannot see the removals.
    /// </summary>
    public FixedTask GetEntityWithJunctions(FixedTask? entity = null)
    {
        entity = GetEntity(entity);

        entity.CategoryFixedTasks.Clear();
        entity.CategoryFixedTasks.AddRange(Categories.Select(catDto => catDto.GetLinkEntity(id => new CategoryFixedTask
        {
            CategoryId = id,
            FixedTaskId = entity.Id
        })));

        return entity;
    }
}
