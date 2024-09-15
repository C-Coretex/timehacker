using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;

namespace TimeHacker.Domain.Contracts.Models.ReturnModels
{
    public class ScheduleEntityReturn: ScheduleEntity
    {
        public IEnumerable<DateOnly> GetNextEntityDatesIn(DateOnly from, DateOnly to)
        {
            var maxIterations = 10_000;
            var nextTaskDate = LastEntityCreated ?? DateOnly.FromDateTime(CreatedTimestamp);

            while (nextTaskDate < to)
            {
                nextTaskDate = RepeatingEntity.RepeatingData.GetNextTaskDate(nextTaskDate);
                if (nextTaskDate > EndsOn || maxIterations-- == 0)
                    yield break;

                if (nextTaskDate >= from && nextTaskDate <= to)
                    yield return nextTaskDate;
            }
        }

        public bool IsEntityDateCorrect(DateOnly date)
        {
            var maxIterations = 10_000;
            var nextTaskDate = DateOnly.FromDateTime(CreatedTimestamp);

            while (nextTaskDate <= date)
            {
                nextTaskDate = RepeatingEntity.RepeatingData.GetNextTaskDate(nextTaskDate);
                if (nextTaskDate > EndsOn || maxIterations-- == 0)
                    return false;

                if (nextTaskDate == date)
                    return true;
            }

            return false;
        }
    }
}
