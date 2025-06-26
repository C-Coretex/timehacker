using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.BusinessLogicExceptions;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IModels;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices.Tasks;

namespace TimeHacker.Domain.Services.Services.Tasks
{
    public class DynamicTaskService : IDynamicTaskService
    {
        private readonly UserAccessorBase _userAccessorBase;
        private readonly IDynamicTaskRepository _dynamicTaskRepository;

        public DynamicTaskService(IDynamicTaskRepository dynamicTaskRepository, UserAccessorBase userAccessorBase)
        {
            _userAccessorBase = userAccessorBase;
            _dynamicTaskRepository = dynamicTaskRepository;
        }

        public async Task AddAsync(DynamicTask task)
        {
            var userId = _userAccessorBase.UserId!;
            task.UserId = userId;

            await _dynamicTaskRepository.AddAndSaveAsync(task);
        }

        public async Task UpdateAsync(DynamicTask task)
        {
            var userId = _userAccessorBase.UserId;

            if (task == null)
                throw new NotProvidedException(nameof(task));


            var oldTask = await _dynamicTaskRepository.GetByIdAsync(task.Id);
            if (oldTask == null)
            {
                await _dynamicTaskRepository.AddAndSaveAsync(task);
                return;
            }

            //TODO: will be removed after repository level filtrations, it will just be null and another exception will be thrown
            if (oldTask.UserId != userId)
                throw new ArgumentException("User can only edit its own tasks.");

            task.UserId = userId;
            await _dynamicTaskRepository.UpdateAndSaveAsync(task);
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                throw new NotFoundException(nameof(task), id.ToString());

            await _dynamicTaskRepository.DeleteAndSaveAsync(task);
        }

        public IQueryable<DynamicTask> GetAll()
        {
            var userId = _userAccessorBase.UserId;
            return _dynamicTaskRepository.GetAll().Where(x => x.UserId == userId);
        }

        public Task<DynamicTask?> GetByIdAsync(Guid id)
        {
            return GetAll().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
