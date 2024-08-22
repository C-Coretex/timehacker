using Microsoft.EntityFrameworkCore;
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

        public async Task DeleteAsync(uint id)
        {
            var userId = _userAccessor.UserId;
            var task = await _fixedTaskRepository.GetByIdAsync(id);
            if (task == null)
                return;

            if (task.UserId != userId)
                throw new ArgumentException("User can only delete its own tasks.");

            await _fixedTaskRepository.DeleteAsync(task);
        }

        public IQueryable<FixedTask> GetAll()
        {
            var userId = _userAccessor.UserId;
            return _fixedTaskRepository.GetAll().Where(x => x.UserId == userId);
        }

        public Task<FixedTask?> GetByIdAsync(uint id)
        {
            return GetByIdAsync(id);
        }

        public async Task UpdateAsync(FixedTask task)
        {
            var userId = _userAccessor.UserId;

            if (task == null || task.Id == 0)
                throw new ArgumentException("Task must be valid");


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
    }
}
