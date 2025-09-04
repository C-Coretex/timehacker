using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Models.ReturnModels;

public record CategoryContainerReturn
{
    public Category? Category { get; init; }
    public TimeRange TimeRange { get; init; }

    //NOT FULLY IMPLEMENTED
    public ScheduledCategory CreateScheduledCategory()
    {
        //fill category data and time range
        return new ScheduledCategory()
        {
            Name = "Scheduled Category",
            Start = TimeRange.Start,
            End = TimeRange.End,
        };
    }

    //NOT FULLY IMPLEMENTED
    public static CategoryContainerReturn Create(ScheduledCategory scheduledCategory)
    {
        return new CategoryContainerReturn
        {
            TimeRange = new TimeRange(scheduledCategory.Start, scheduledCategory.End),
            Category = new Category
            {
                UserId = scheduledCategory.UserId,
                ScheduleEntityId = scheduledCategory.ParentCategoryId,
                Name = scheduledCategory.Name,
                Description = scheduledCategory.Description,
                Color = scheduledCategory.Color
            }
        };
    }
}