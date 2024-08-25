namespace TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots
{
    public class EndsOnModel
    {
        public DateOnly? MaxDate { get; set; }
        public uint? MaxOccurrences { get; set; }
    }
}
