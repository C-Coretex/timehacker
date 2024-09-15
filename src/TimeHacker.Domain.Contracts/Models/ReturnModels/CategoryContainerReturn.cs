using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Models.BusinessLogicModels;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels;

public class CategoryContainerReturn
{
    public Category? Category { get; set; }
    public TimeRange TimeRange { get; set; }
}