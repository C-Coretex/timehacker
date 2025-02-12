using AutoMapper;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.IncludeExpansionDelegates;

namespace TimeHacker.Domain.Services.ScheduleSnapshots
{
    public class ScheduleEntityService : IScheduleEntityService
    {
        private readonly IScheduleEntityRepository _scheduleEntityRepository;
        private readonly IFixedTaskService _fixedTaskService;
        private readonly ICategoryService _categoryService;

        private readonly UserAccessorBase _userAccessorBase;
        private readonly IMapper _mapper;

        public ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository, IFixedTaskService fixedTaskService, ICategoryService categoryService, UserAccessorBase userAccessorBase, IMapper mapper)
        {
            _scheduleEntityRepository = scheduleEntityRepository;
            _fixedTaskService = fixedTaskService;
            _categoryService = categoryService;

            _userAccessorBase = userAccessorBase;
            _mapper = mapper;
        }


        public IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from)
        {
            var query = _scheduleEntityRepository.GetAll(IncludeExpansionScheduleEntity.IncludeFixedTask)
                .Where(x => x.UserId == _userAccessorBase.UserId && (x.EndsOn == null || x.EndsOn >= from));

            return _mapper.ProjectTo<ScheduleEntityReturn>(query);
        }

        public async Task UpdateLastEntityCreated(Guid id, DateOnly entityCreated)
        {
            var scheduleEntity = await _scheduleEntityRepository.GetByIdAsync(id);
            if (scheduleEntity == null)
                throw new Exception("ScheduleEntity is not found.");

            if (scheduleEntity.UserId != _userAccessorBase.UserId)
                throw new Exception("User can edit only his own ScheduleEntity.");

            var scheduleEntityReturn = _mapper.Map<ScheduleEntityReturn>(scheduleEntity);
            if (!scheduleEntityReturn.IsEntityDateCorrect(entityCreated))
                throw new ArgumentException("Created entity timestamp is not correct for this ScheduleEntityReturn.", nameof(entityCreated));

            if (scheduleEntity.LastEntityCreated != null && scheduleEntity.LastEntityCreated >= entityCreated)
                return;

            scheduleEntity.LastEntityCreated = entityCreated;
            await _scheduleEntityRepository.UpdateAsync(scheduleEntity);
        }

        public Task<ScheduleEntity> Save(InputScheduleEntityModel inputScheduleEntity)
        {
            var scheduleEntity = inputScheduleEntity.GetScheduleEntity();
            scheduleEntity.UserId = _userAccessorBase.UserId!;

            return inputScheduleEntity.ScheduleEntityParentEnum switch
            {
                ScheduleEntityParentEnum.FixedTask => _fixedTaskService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                ScheduleEntityParentEnum.Category => _categoryService.UpdateScheduleEntityAsync(scheduleEntity, inputScheduleEntity.ParentEntityId),
                _ => throw new ArgumentException("ScheduleEntityParent must be chosen", nameof(ScheduleEntityParentEnum)),
            };
        }
    }
}
