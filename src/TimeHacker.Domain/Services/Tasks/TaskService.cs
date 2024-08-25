using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.Processors;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeHacker.Domain.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly IScheduleSnapshotService _scheduleSnapshotService;
        private readonly IScheduleEntityService _scheduleEntityService;

        private readonly IMapper _mapper;

        private readonly TaskTimelineProcessor _taskTimelineProcessor;

        public TaskService(IFixedTaskService fixedTaskService, IDynamicTaskService dynamicTaskService, IScheduleSnapshotService scheduleSnapshotService, IMapper mapper, IScheduleEntityService scheduleEntityService)
        {
            _fixedTaskService = fixedTaskService;
            _dynamicTaskService = dynamicTaskService;
            _scheduleSnapshotService = scheduleSnapshotService;
            _scheduleEntityService = scheduleEntityService;

            _mapper = mapper;

            _taskTimelineProcessor = new TaskTimelineProcessor();
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateOnly date)
        {
            var snapshot = await _scheduleSnapshotService.GetBy(date);
            if (snapshot != null)
                return _mapper.Map<TasksForDayReturn>(snapshot);

            var fixedTasks = await _fixedTaskService.GetAll()
                                              .Where(ft => DateOnly.FromDateTime(ft.StartTimestamp) == date)
                                              .OrderBy(ft => ft.StartTimestamp)
                                              .ToListAsync();

            var dynamicTasks = _dynamicTaskService.GetAll().AsEnumerable();


            fixedTasks.AddRange(await ExtendFixedTasksWithScheduledTasks(date).ToListAsync());
            fixedTasks = fixedTasks.OrderBy(ft => ft.StartTimestamp).ToList();

            var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasks, dynamicTasks, date);

            snapshot = _mapper.Map<ScheduleSnapshot>(tasksForDay);
            snapshot = await _scheduleSnapshotService.Add(snapshot);

            return _mapper.Map<TasksForDayReturn>(snapshot);
        }

        public async IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(IEnumerable<DateOnly> dates)
        {
            dates = dates.ToList();
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            fixedTasks.AddRange(await ExtendFixedTasksWithScheduledTasks(dates.Min(), dates.Max()).ToListAsync());
            fixedTasks = fixedTasks.OrderBy(ft => ft.StartTimestamp).ToList();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(date);
                if (snapshot != null)
                {
                    yield return _mapper.Map<TasksForDayReturn>(snapshot);
                }
                else
                {
                    var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                    var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                    snapshot = _mapper.Map<ScheduleSnapshot>(tasksForDay);
                    snapshot = await _scheduleSnapshotService.Add(snapshot);

                    yield return _mapper.Map<TasksForDayReturn>(snapshot);
                }
            }
        }

        public async IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(IEnumerable<DateOnly> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            fixedTasks.AddRange(await ExtendFixedTasksWithScheduledTasks(dates.Min(), dates.Max()).ToListAsync());
            fixedTasks = fixedTasks.OrderBy(ft => ft.StartTimestamp).ToList();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(date) ?? new ScheduleSnapshot();

                var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                _mapper.Map(tasksForDay, snapshot);

                if (snapshot.UserId == null)
                    snapshot = await _scheduleSnapshotService.Add(snapshot);
                else
                    snapshot = await _scheduleSnapshotService.Update(snapshot);

                yield return _mapper.Map<TasksForDayReturn>(snapshot);
            }
        }

        private async IAsyncEnumerable<FixedTask> ExtendFixedTasksWithScheduledTasks(DateOnly from, DateOnly? to = null)
        {
            var scheduleEntities = _scheduleEntityService.GetAllFrom(from).AsAsyncEnumerable();

            await foreach (var scheduleEntity in scheduleEntities)
            {
                var taskDates = scheduleEntity.GetNextTaskDatesIn(from, to ?? from);

                foreach (var taskDate in taskDates)
                {
                    var task = scheduleEntity.FixedTask!.ShallowCopy();
                    var timeDifference = task.EndTimestamp.Date - task.StartTimestamp.Date;

                    task.StartTimestamp = taskDate.ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));
                    task.EndTimestamp = taskDate.AddDays(timeDifference.Days).ToDateTime(TimeOnly.FromDateTime(task.StartTimestamp));

                    yield return task;
                }
            }
        }
    }
}
