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
        private readonly IUserAccessor _userAccessor;
        private readonly IFixedTaskRepository _fixedTaskRepository;

        public FixedTaskService(IFixedTaskRepository FixedTaskRepository, IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
            _fixedTaskRepository = FixedTaskRepository;
        }

        public async Task AddAsync(FixedTask task)
        {
            var userId = _userAccessor.UserId!;
            task.UserId = userId;

            await _fixedTaskRepository.AddAsync(task);
        }

        public async Task UpdateAsync(FixedTask task)
        {
            if (task == null || task.Id == 0)
                throw new ArgumentException("Task must be valid");

            var userId = _userAccessor.UserId;

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

        public async Task DeleteAsync(uint id)
        {
            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                throw new ArgumentException("Task by this Id is not found for current user.");

            await _fixedTaskRepository.DeleteAsync(task);
        }

        public IQueryable<FixedTask> GetAll() => GetAll(true);

        public Task<FixedTask?> GetByIdAsync(uint id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ScheduleEntity> UpdateScheduleEntityAsync(ScheduleEntity scheduleEntity, uint taskId)
        {
            if (scheduleEntity == null || taskId == 0)
                throw new ArgumentException("Values are incorrect.");

            var task = await GetAll(false).FirstOrDefaultAsync(x => x.Id == taskId);
            if (task == null)
                throw new ArgumentException("Task by this Id is not found for current user.");

            task.ScheduleEntity = scheduleEntity;
            return (await _fixedTaskRepository.UpdateAsync(task)).ScheduleEntity!;
        }

        private IQueryable<FixedTask> GetAll(bool asNoTracking)
        {
            var userId = _userAccessor.UserId;
            return _fixedTaskRepository.GetAll(asNoTracking).Where(x => x.UserId == userId);
        }
    }
}
