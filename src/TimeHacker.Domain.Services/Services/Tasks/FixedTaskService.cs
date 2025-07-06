using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;

namespace TimeHacker.Domain.Services.Services.Tasks
{
    public class FixedTaskService : IFixedTaskService
    {
        private readonly UserAccessorBase _userAccessorBase;
        private readonly IFixedTaskRepository _fixedTaskRepository;

        public FixedTaskService(IFixedTaskRepository FixedTaskRepository, UserAccessorBase userAccessorBase)
        {
            _userAccessorBase = userAccessorBase;
            _fixedTaskRepository = FixedTaskRepository;
        }

        public async Task AddAsync(FixedTask task)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            task.UserId = userId;

            await _fixedTaskRepository.AddAndSaveAsync(task);
        }

        public async Task UpdateAsync(FixedTask task)
        {
            if (task == null)
                throw new NotProvidedException(nameof(task));

            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();

            var oldTask = await _fixedTaskRepository.GetByIdAsync(task.Id);
            if (oldTask == null)
            {
                await _fixedTaskRepository.AddAndSaveAsync(task);
                return;
            }

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (oldTask.UserId != userId)
                throw new ArgumentException("User can only edit its own tasks.");

            task.UserId = userId;
            await _fixedTaskRepository.UpdateAndSaveAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                throw new NotFoundException(nameof(task), id.ToString());

            await _fixedTaskRepository.DeleteAndSaveAsync(task);
        }

        public IQueryable<FixedTask> GetAll() => GetAll(true);

        public Task<FixedTask?> GetByIdAsync(Guid id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid taskId)
        {
            if (scheduleEntity == null)
                throw new NotProvidedException(nameof(scheduleEntity));

            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null)
                throw new NotFoundException(nameof(task), taskId.ToString());

            task.ScheduleEntity = scheduleEntity;
            return (await _fixedTaskRepository.UpdateAndSaveAsync(task)).ScheduleEntity!;
        }

        private IQueryable<FixedTask> GetAll(bool asNoTracking)
        {
            var userId = _userAccessorBase.GetUserIdOrThrowUnauthorized();
            return _fixedTaskRepository.GetAll(asNoTracking).Where(x => x.UserId == userId);
        }
    }
}
