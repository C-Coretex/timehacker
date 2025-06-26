using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.IServices.Tasks;
using TimeHacker.Domain.Models.ReturnModels;
using TimeHacker.Domain.Processors;

namespace TimeHacker.Domain.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly IScheduleSnapshotService _scheduleSnapshotService;
        private readonly IScheduleEntityService _scheduleEntityService;


        private readonly TaskTimelineProcessor _taskTimelineProcessor;

        public TaskService(
            IFixedTaskService fixedTaskService, 
            IDynamicTaskService dynamicTaskService, 
            IScheduleSnapshotService scheduleSnapshotService, 
            IScheduleEntityService scheduleEntityService)
        {
            _fixedTaskService = fixedTaskService;
            _dynamicTaskService = dynamicTaskService;
            _scheduleSnapshotService = scheduleSnapshotService;
            _scheduleEntityService = scheduleEntityService;

            _taskTimelineProcessor = new TaskTimelineProcessor();
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateOnly date)
        {
            var snapshot = await _scheduleSnapshotService.GetByAsync(date);
            if (snapshot != null)
                return TasksForDayReturn.Create(snapshot);

            var fixedTasks = await _fixedTaskService.GetAll()
                                              .Where(ft => DateOnly.FromDateTime(ft.StartTimestamp) == date)
                                              .OrderBy(ft => ft.StartTimestamp)
                                              .ToListAsync();

            var dynamicTasks = _dynamicTaskService.GetAll().AsEnumerable();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(date).ToListAsync();

            var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasks, scheduledFixedTasks, dynamicTasks, date);

            snapshot = tasksForDay.CreateScheduleSnapshot();
            snapshot = await _scheduleSnapshotService.AddAsync(snapshot);

            return TasksForDayReturn.Create(snapshot);
        }

        public async IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(ICollection<DateOnly> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max()).ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetByAsync(date);
                if (snapshot != null)
                    yield return TasksForDayReturn.Create(snapshot);
                else
                {
                    var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                    var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                    var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

                    snapshot = tasksForDay.CreateScheduleSnapshot();
                    snapshot = await _scheduleSnapshotService.AddAsync(snapshot);

                    yield return TasksForDayReturn.Create(snapshot);
                }
            }
        }

        public async IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(ICollection<DateOnly> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d == DateOnly.FromDateTime(ft.StartTimestamp)))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            var scheduledFixedTasks = await GetFixedTasksForScheduledTasks(dates.Min(), dates.Max()).ToListAsync();

            foreach (var date in dates)
            {
                var insert = false;
                var snapshot = await _scheduleSnapshotService.GetByAsync(date);
                if (snapshot == null)
                {
                    snapshot = new ScheduleSnapshot();
                    insert = true;
                }

                var fixedTasksForDay = fixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date);
                var scheduledFixedTasksForDay = scheduledFixedTasks.Where(ft => DateOnly.FromDateTime(ft.StartTimestamp.Date) == date).ToList();
                var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, scheduledFixedTasksForDay, dynamicTasks, date);

                snapshot = tasksForDay.CreateScheduleSnapshot(snapshot);

                if (insert)
                    snapshot = await _scheduleSnapshotService.AddAsync(snapshot);
                else
                    snapshot = await _scheduleSnapshotService.UpdateAsync(snapshot);

                foreach(var scheduledFixedTasksForDayEntry in scheduledFixedTasksForDay)
                {
                    await _scheduleEntityService.UpdateLastEntityCreated(scheduledFixedTasksForDayEntry.ScheduleEntityId!.Value,
                        date);
                }

                yield return TasksForDayReturn.Create(snapshot);
            }
        }

        private async IAsyncEnumerable<FixedTask> GetFixedTasksForScheduledTasks(DateOnly from, DateOnly? to = null)
        {
            var scheduleEntities = _scheduleEntityService.GetAllFrom(from).AsAsyncEnumerable();

            await foreach (var scheduleEntity in scheduleEntities)
            {
                var taskDates = scheduleEntity.GetNextEntityDatesIn(from, to ?? from);

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
