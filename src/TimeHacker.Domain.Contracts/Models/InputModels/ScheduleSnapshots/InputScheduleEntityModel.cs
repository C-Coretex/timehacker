using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;

namespace TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots
{
    public class InputScheduleEntityModel
    {
        public ScheduleEntityParentEnum ScheduleEntityParentEnum { get; set; }
        public uint ParentEntityId { get; set; }

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
                scheduleEntity.EndsOn = EndsOnModel.MaxDate;
                if (EndsOnModel.MaxOccurrences != null)
                {
                    var repeatingEntity = (IRepeatingEntityType) RepeatingEntityModel.RepeatingData;
                    var date = DateOnly.FromDateTime(DateTime.UtcNow);
                    for (var i = 0; i < EndsOnModel.MaxOccurrences; i++)
                        date = repeatingEntity.GetNextTaskDate(date);

                    scheduleEntity.EndsOn = date;
                }
            }

            return scheduleEntity;
        }
    }
}
