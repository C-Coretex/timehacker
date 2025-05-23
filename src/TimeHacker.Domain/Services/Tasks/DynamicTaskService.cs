﻿using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.Tasks;

namespace TimeHacker.Domain.Services.Tasks
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

            await _dynamicTaskRepository.AddAsync(task);
        }

        public async Task UpdateAsync(DynamicTask task)
        {
            var userId = _userAccessorBase.UserId;

            if (task == null)
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

        public async Task DeleteAsync(Guid id)
        {
            var task = await GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (task == null)
                throw new ArgumentException("Task by this Id is not found for current user.");

            await _dynamicTaskRepository.DeleteAsync(task);
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
