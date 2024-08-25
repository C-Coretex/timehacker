using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public class ScheduleEntityReturn: ScheduleEntity
    {
        public IEnumerable<DateOnly> GetNextTaskDatesIn(DateOnly from, DateOnly to)
        {
            var maxIterations = 10_000;
            var nextTaskDate = DateOnly.FromDateTime(LastTaskCreated);

            while (nextTaskDate < to)
            {
                nextTaskDate = RepeatingEntity.RepeatingData.GetNextTaskDate(nextTaskDate);
                if (nextTaskDate > EndsOn || maxIterations-- == 0)
                    yield break;

                if (nextTaskDate >= from && nextTaskDate <= to)
                    yield return nextTaskDate;
            }
        }
    }
}
