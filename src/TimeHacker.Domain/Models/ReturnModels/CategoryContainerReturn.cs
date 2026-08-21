using TimeHacker.Domain.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Models.ReturnModels;

public record CategoryContainerReturn
{
    public Guid? ScheduleEntityId { get; init; }
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
            Color = Category.Color,
            ParentCategoryId = Category.Id,
            ParentScheduleEntity = ScheduleEntityId
        };
    }

    /// <summary>
    /// Rebuilds a container from a persisted <see cref="ScheduledCategory"/> snapshot row. The snapshot is
    /// denormalized, so the originating <see cref="Category"/> is NOT reloaded — only a thin shell carrying
    /// the captured display fields (name, description, colour) is reconstructed, keyed by ParentCategoryId.
    /// </summary>
    public static CategoryContainerReturn Create(ScheduledCategory scheduledCategory)
    {
        ArgumentNullException.ThrowIfNull(scheduledCategory);
        return new CategoryContainerReturn
        {
            ScheduleEntityId = scheduledCategory.ParentScheduleEntity,
            TimeRange = new TimeRange(scheduledCategory.Start, scheduledCategory.End),
            Category = new Category
            {
                Id = scheduledCategory.ParentCategoryId,
                UserId = scheduledCategory.UserId,
                Name = scheduledCategory.Name,
                Description = scheduledCategory.Description,
                Color = scheduledCategory.Color,
                StartTime = TimeOnly.FromTimeSpan(scheduledCategory.Start),
                EndTime = TimeOnly.FromTimeSpan(scheduledCategory.End)
            }
        };
    }
}
