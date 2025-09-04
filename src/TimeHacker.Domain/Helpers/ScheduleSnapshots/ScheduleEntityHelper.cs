using TimeHacker.Domain.DTOs.RepeatingEntity;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Domain.Helpers.ScheduleSnapshots
{
    public static class ScheduleEntityHelper
    {
        public static ScheduleEntity GetScheduleEntity(RepeatingEntityDto repeatingEntityModel, EndsOnModel? endsOnModel)
        {
            var scheduleEntity = new ScheduleEntity
            {
                RepeatingEntity = repeatingEntityModel
            };

            if (endsOnModel == null) 
                return scheduleEntity;

            if (endsOnModel.MaxOccurrences != null)
            {
                var date = DateOnly.FromDateTime(DateTime.UtcNow);
                for (var i = 0; (i < endsOnModel.MaxOccurrences && date < endsOnModel.MaxDate.GetValueOrDefault(DateOnly.MaxValue)); i++)
                    date = repeatingEntityModel.RepeatingData.GetNextTaskDate(date);

                scheduleEntity.EndsOn = date;
            }

            if (scheduleEntity.EndsOn == null || scheduleEntity.EndsOn > endsOnModel.MaxDate)
                scheduleEntity.EndsOn = endsOnModel.MaxDate;

            return scheduleEntity;
        }
    }
}
