using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduleEntityService : IScheduleEntityService
    {
        private readonly IScheduleEntityRepository _scheduleEntityRepository;
        private readonly IFixedTaskService _fixedTaskService;
        private readonly ICategoryService _categoryService;

        private readonly IUserAccessor _userAccessor;

        public ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository, IFixedTaskService fixedTaskService, ICategoryService categoryService, IUserAccessor userAccessor)
        {
            _scheduleEntityRepository = scheduleEntityRepository;
            _fixedTaskService = fixedTaskService;
            _categoryService = categoryService;

            _userAccessor = userAccessor;
        }


        public IQueryable<ScheduleEntity> GetAll(bool onlyActive)
        {
            var query = _scheduleEntityRepository.GetAll().Where(x => x.UserId == _userAccessor.UserId);
            if(onlyActive)
                query = query.Where(x => x.EndsOn >= DateOnly.FromDateTime(DateTime.UtcNow));

            return query;
        }

        public Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity)
        {
            var scheduleEntity = inputScheduleEntity.GetScheduleEntity();
            scheduleEntity.UserId = _userAccessor.UserId!;

            return inputScheduleEntity.ScheduleEntityParentEnum switch
            {
                ScheduleEntityParentEnum.FixedTask => _fixedTaskService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                ScheduleEntityParentEnum.Category => _categoryService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                _ => throw new ArgumentException("ScheduleEntityParent must be chosen", nameof(ScheduleEntityParentEnum)),
            };
        }

        public Task Delete(uint id)
        {
            return _scheduleEntityRepository.DeleteAsync(id);
        }
    }
}
