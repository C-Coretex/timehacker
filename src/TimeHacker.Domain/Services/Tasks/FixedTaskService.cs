using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Domain.Services.Tasks
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
            var userId = _userAccessorBase.UserId!;
            task.UserId = userId;

            await _fixedTaskRepository.AddAsync(task);
        }

        public async Task UpdateAsync(FixedTask task)
        {
            if (task == null)
                throw new ArgumentException("Task must be valid");

            var userId = _userAccessorBase.UserId;

            var oldTask = await _fixedTaskRepository.GetByIdAsync(task.Id);
            if (oldTask == null)
            {
                await _fixedTaskRepository.AddAsync(task);
                return;
            }

            if (oldTask.UserId != userId)
                throw new ArgumentException("User can only edit its own tasks.");

            task.UserId = userId;
            await _fixedTaskRepository.UpdateAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                throw new ArgumentException("Task by this Id is not found for current user.");

            await _fixedTaskRepository.DeleteAsync(task);
        }

        public IQueryable<FixedTask> GetAll() => GetAll(true);

        public Task<FixedTask?> GetByIdAsync(Guid id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, Guid taskId)
        {
            if (scheduleEntity == null)
                throw new ArgumentException("Values are incorrect.");

            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null)
                throw new ArgumentException("Task by this Id is not found for current user.");

            task.ScheduleEntity = scheduleEntity;
            return (await _fixedTaskRepository.UpdateAsync(task)).ScheduleEntity!;
        }

        private IQueryable<FixedTask> GetAll(bool asNoTracking)
        {
            var userId = _userAccessorBase.UserId;
            return _fixedTaskRepository.GetAll(asNoTracking).Where(x => x.UserId == userId);
        }
    }
}
