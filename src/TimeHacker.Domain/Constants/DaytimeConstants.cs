namespace TimeHacker.Domain.Constants
{
    public static class DaytimeConstants
    {
        public static readonly TimeSpan StartOfDay = new(0, 0, 0);
        public static readonly TimeSpan EndOfDay = new(23, 59, 59);
        public static readonly TimeSpan TimeBacklashBetweenTasks = new(0, 5, 0);
    }
}
