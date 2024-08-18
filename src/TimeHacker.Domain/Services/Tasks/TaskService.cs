using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.Processors;

namespace TimeHacker.Domain.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly IScheduleSnapshotService _scheduleSnapshotService;

        private readonly IMapper _mapper;

        private readonly TaskTimelineProcessor _taskTimelineProcessor;

        public TaskService(IFixedTaskService fixedTaskService, IDynamicTaskService dynamicTaskService, IScheduleSnapshotService scheduleSnapshotService, IMapper mapper)
        {
            _fixedTaskService = fixedTaskService;
            _dynamicTaskService = dynamicTaskService;
            _scheduleSnapshotService = scheduleSnapshotService;

            _mapper = mapper;

            _taskTimelineProcessor = new TaskTimelineProcessor();
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateTime date)
        {
            var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date));
            if(snapshot != null)
                return _mapper.Map<TasksForDayReturn>(snapshot);

            var fixedTasks = _fixedTaskService.GetAll()
                                              .Where(ft => ft.StartTimestamp.Date == date.Date)
                                              .OrderBy(ft => ft.StartTimestamp)
                                              .AsEnumerable();

            var dynamicTasks = _dynamicTaskService.GetAll().AsEnumerable();

            var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasks, dynamicTasks, date);

            snapshot = _mapper.Map<ScheduleSnapshot>(tasksForDay);
            snapshot = await _scheduleSnapshotService.Add(snapshot);

            return _mapper.Map<TasksForDayReturn>(snapshot);
        }

        public async IAsyncEnumerable<TasksForDayReturn> GetTasksForDays(IEnumerable<DateTime> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d.Date == ft.StartTimestamp.Date))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date));
                if (snapshot != null)
                {
                    yield return _mapper.Map<TasksForDayReturn>(snapshot);
                }
                else
                {
                    var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                    var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                    snapshot = _mapper.Map<ScheduleSnapshot>(tasksForDay);
                    snapshot = await _scheduleSnapshotService.Add(snapshot);

                    yield return _mapper.Map<TasksForDayReturn>(snapshot);
                }
            }
        }

        public async IAsyncEnumerable<TasksForDayReturn> RefreshTasksForDays(IEnumerable<DateTime> dates)
        {
            var fixedTasks = await _fixedTaskService.GetAll()
                                                    .Where(ft => dates.Any(d => d.Date == ft.StartTimestamp.Date))
                                                    .OrderBy(ft => ft.StartTimestamp)
                                                    .ToListAsync();

            var dynamicTasks = await _dynamicTaskService.GetAll().ToListAsync();

            foreach (var date in dates)
            {
                var snapshot = await _scheduleSnapshotService.GetBy(DateOnly.FromDateTime(date)) ?? new ScheduleSnapshot();

                var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                var tasksForDay = _taskTimelineProcessor.GetTasksForDay(fixedTasksForDay, dynamicTasks, date);

                _mapper.Map(tasksForDay, snapshot);

                if (snapshot.UserId == null)
                    snapshot = await _scheduleSnapshotService.Add(snapshot);
                else
                    snapshot = await _scheduleSnapshotService.Update(snapshot);

                yield return _mapper.Map<TasksForDayReturn>(snapshot);
            }
        }
    }
}
