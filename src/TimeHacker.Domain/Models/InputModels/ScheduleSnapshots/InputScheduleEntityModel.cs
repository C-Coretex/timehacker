using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Models.EntityModels;

namespace TimeHacker.Domain.Models.InputModels.ScheduleSnapshots
{
    public class InputScheduleEntityModel
    {
        public ScheduleEntityParentEnum ScheduleEntityParentEnum { get; set; }
        public Guid ParentEntityId { get; set; }

        public RepeatingEntityModel RepeatingEntityModel { get; set; }
        public EndsOnModel? EndsOnModel { get; set; }


        public ScheduleEntity GetScheduleEntity()
        {
            var scheduleEntity = new ScheduleEntity
            {
                RepeatingEntity = RepeatingEntityModel
            };

            if (EndsOnModel != null)
            {
                if (EndsOnModel.MaxOccurrences != null)
                {
                    var date = DateOnly.FromDateTime(DateTime.UtcNow);
                    for (var i = 0; i < EndsOnModel.MaxOccurrences; i++)
                        date = RepeatingEntityModel.RepeatingData.GetNextTaskDate(date);

                    scheduleEntity.EndsOn = date;
                }

                if(scheduleEntity.EndsOn == null || scheduleEntity.EndsOn > EndsOnModel.MaxDate)
                    scheduleEntity.EndsOn = EndsOnModel.MaxDate;
            }

            return scheduleEntity;
        }
    }
}
