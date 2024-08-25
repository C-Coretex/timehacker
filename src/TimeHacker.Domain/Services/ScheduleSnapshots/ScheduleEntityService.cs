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

        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public ScheduleEntityService(IScheduleEntityRepository scheduleEntityRepository, IFixedTaskService fixedTaskService, ICategoryService categoryService, IUserAccessor userAccessor, IMapper mapper)
        {
            _scheduleEntityRepository = scheduleEntityRepository;
            _fixedTaskService = fixedTaskService;
            _categoryService = categoryService;

            _userAccessor = userAccessor;
            _mapper = mapper;
        }


        public IQueryable<ScheduleEntityReturn> GetAllFrom(DateOnly from)
        {
            var query = _mapper.ProjectTo<ScheduleEntityReturn>(
                _scheduleEntityRepository.GetAll(IncludeExpansionScheduleEntity.IncludeFixedTask)
                                               .Where(x => x.UserId == _userAccessor.UserId && x.EndsOn >= from)
            );

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
