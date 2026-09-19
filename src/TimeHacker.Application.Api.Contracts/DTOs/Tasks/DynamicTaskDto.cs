using TimeHacker.Application.Api.Contracts.DTOs.Categories;
using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Helpers.Domain.Extensions;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks;

public record DynamicTaskDto()
{
    public Guid? Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public byte Priority { get; init; }
    public TimeSpan MinTimeToFinish { get; init; }
    public TimeSpan MaxTimeToFinish { get; init; }
    public TimeSpan? OptimalTimeToFinish { get; init; }
    public DateTime CreatedTimestamp { get; init; }
    //TODO: we can move it to Union probably.
    //Or maybe to a separate property that holds rich and non rich and synchronizes them automatically
    public ICollection<LinkCategoryDto> Categories { get; init => field = [.. value.DistinctBy(c => c.Id)]; } = [];
    public IEnumerable<CategoryDto> RichCategories => Categories.OfType<CategoryDto>();
    public IEnumerable<TagDto> Tags { get; init; } = [];

    public static Expression<Func<DynamicTask, DynamicTaskDto>> Selector =>
        x => new DynamicTaskDto()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Priority = x.Priority,
            MinTimeToFinish = x.MinTimeToFinish,
            MaxTimeToFinish = x.MaxTimeToFinish,
            OptimalTimeToFinish = x.OptimalTimeToFinish,
            CreatedTimestamp = x.CreatedTimestamp,
            // AsQueryable keeps CategoryDto.Selector visible to the translator, so each category's Schedules
            // are projected too; CategoryDto.Create compiles just as well but its opaque Func hides them from
            // EF and they come back silently empty. ToList: EF rejects an IQueryable in a final projection.
            //TODO: check if it's really needed (it's needed to preserve ScheduledEntitties - navigation property of the Category). If it's really needed we need to implement it everywhere
            Categories = x.CategoryDynamicTasks.AsQueryable().Select(categoryTask => categoryTask.Category).Select(CategoryDto.Selector)
                .OfType<LinkCategoryDto>().ToList(),
            Tags = x.TagDynamicTasks.Select(tagTask => TagDto.Create(tagTask.Tag))
        };

    private static readonly Func<DynamicTask, DynamicTaskDto> CreateFunc = Selector.Compile();
    public static DynamicTaskDto? Create(DynamicTask? entity) => entity != null ? CreateFunc(entity) : null;

    public DynamicTask GetEntity(DynamicTask? entity = null)
    {
        entity ??= new DynamicTask();

        entity.Name = Name;
        entity.Description = Description;
        entity.Priority = Priority;
        entity.MinTimeToFinish = MinTimeToFinish;
        entity.MaxTimeToFinish = MaxTimeToFinish;
        entity.OptimalTimeToFinish = OptimalTimeToFinish;

        return entity;
    }

    /// <summary>
    /// <see cref="GetEntity"/> plus the category junctions, replacing whatever the entity currently
    /// carries. The entity must be loaded with its junctions or EF cannot see the removals.
    /// </summary>
    public DynamicTask GetEntityWithJunctions(DynamicTask? entity = null)
    {
        entity = GetEntity(entity);

        entity.CategoryDynamicTasks.Clear();
        entity.CategoryDynamicTasks.AddRange(Categories.Select(catDto => catDto.GetLinkEntity(id => new CategoryDynamicTask
        {
            CategoryId = id,
            DynamicTaskId = entity.Id
        })));

        return entity;
    }
}
