using TimeHacker.Domain.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Models.ReturnModels;

public record CategoryContainerReturn
{
    public Guid? ScheduleEntityId { get; init; }
    public Guid CategoryScheduleId { get; init; }

    /// <summary>The window's own optional note; the name and colour come from <see cref="Category"/>.</summary>
    public string? ScheduleDescription { get; init; }

    /// <summary>The parent category tasks link to, carrying its id, name, description and colour.</summary>
    public required Category Category { get; init; }

    public TimeRange TimeRange { get; init; }

    public ScheduledCategory CreateScheduledCategory()
    {
        return new ScheduledCategory()
        {
            Start = TimeRange.Start,
            End = TimeRange.End,
            UserId = Category.UserId,
            Name = Category.Name,
            Description = Category.Description,
            ScheduleDescription = ScheduleDescription,
            Color = Category.Color,
            ParentCategoryId = Category.Id,
            ParentCategoryScheduleId = CategoryScheduleId,
            ParentScheduleEntity = ScheduleEntityId
        };
    }

    /// <summary>
    /// Rebuilds a container from a persisted <see cref="ScheduledCategory"/> snapshot row. The snapshot is
    /// denormalized, so neither the originating window nor its category is reloaded — everything needed is
    /// already on the row, and it reads back even if the originals have since been deleted.
    /// </summary>
    public static CategoryContainerReturn Create(ScheduledCategory scheduledCategory)
    {
        ArgumentNullException.ThrowIfNull(scheduledCategory);

        return new CategoryContainerReturn
        {
            ScheduleEntityId = scheduledCategory.ParentScheduleEntity,
            CategoryScheduleId = scheduledCategory.ParentCategoryScheduleId,
            ScheduleDescription = scheduledCategory.ScheduleDescription,
            TimeRange = new TimeRange(scheduledCategory.Start, scheduledCategory.End),
            Category = new Category
            {
                Id = scheduledCategory.ParentCategoryId,
                UserId = scheduledCategory.UserId,
                Name = scheduledCategory.Name,
                Description = scheduledCategory.Description,
                Color = scheduledCategory.Color
            }
        };
    }
}
