using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Domain.Services.Tasks
{
    public class DynamicTaskService : IDynamicTaskService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IDynamicTaskRepository _dynamicTaskRepository;

        public DynamicTaskService(IUserAccessor userAccessor, IDynamicTaskRepository dynamicTaskRepository)
        {
            _userAccessor = userAccessor;
            _dynamicTaskRepository = dynamicTaskRepository;
        }

        public async Task AddAsync(DynamicTask task)
        {
            var userId = _userAccessor.UserId!;
            task.UserId = userId;

            await _dynamicTaskRepository.AddAsync(task);
        }

        public async Task DeleteAsync(int id)
        {
            var userId = _userAccessor.UserId;
            var task = await _dynamicTaskRepository.GetByIdAsync(id);
            if (task == null)
                return;

            if (task.UserId != userId)
                throw new ArgumentException("User can only delete its own tasks.");

            await _dynamicTaskRepository.DeleteAsync(task);
        }

        public IQueryable<DynamicTask> GetAll()
        {
            var userId = _userAccessor.UserId;
            return _dynamicTaskRepository.GetAll().Where(x => x.UserId == userId);
        }

        public Task<DynamicTask?> GetByIdAsync(int id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(DynamicTask task)
        {
            var userId = _userAccessor.UserId;

            if (task == null || task.Id == 0)
                throw new ArgumentException("Task must be valid");


            var oldTask = await _dynamicTaskRepository.GetByIdAsync(task.Id);
            if (oldTask == null)
            {
                await _dynamicTaskRepository.AddAsync(task);
                return;
            }

            if (oldTask.UserId != userId)
                throw new ArgumentException("User can only edit its own tasks.");

            task.UserId = userId;
            await _dynamicTaskRepository.UpdateAsync(task);
        }
    }
}
