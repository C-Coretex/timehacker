namespace TimeHacker.Domain.Models.BusinessLogicModels;

public struct TimeRange
{
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }

    public TimeRange() { }
    public TimeRange(TimeSpan start, TimeSpan end)
    {
        Start = start;
        End = end;
    }
}
