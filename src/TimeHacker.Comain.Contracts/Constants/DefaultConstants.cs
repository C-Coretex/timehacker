namespace TimeHacker.Domain.Contracts.Constants
{
    public static class DefaultConstants
    {
        public static readonly TimeSpan StartOfDay = new(0, 0, 0);
        public static readonly TimeSpan EndOfDay = new(23, 59, 59);
        public static readonly TimeSpan TimeBacklashBetweenTasks = new(0, 5, 0);
    }
}
