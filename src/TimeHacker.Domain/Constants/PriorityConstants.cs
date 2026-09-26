namespace TimeHacker.Domain.Constants;

/// <summary>Bounds of the task priority scale. Lower numbers are more important, which is what the scheduler favours.</summary>
public static class PriorityConstants
{
    public const byte Highest = 1;
    public const byte Lowest = 5;
    public const byte Default = 3;
}
