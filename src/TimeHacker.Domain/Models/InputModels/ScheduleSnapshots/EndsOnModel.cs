namespace TimeHacker.Domain.Models.InputModels.ScheduleSnapshots
{
    public class EndsOnModel
    {
        public DateOnly? MaxDate { get; set; }
        public uint? MaxOccurrences { get; set; }
    }
}
