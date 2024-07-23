using Microsoft.EntityFrameworkCore;
using TimeHacker.Domain.Contracts.IServices.Tasks;
using TimeHacker.Domain.Contracts.Models.ReturnModels;
using TimeHacker.Domain.Processors;

namespace TimeHacker.Domain.Services.Tasks
{
    public class TaskService : ITaskService
    {
        private readonly IFixedTaskService _fixedTaskService;
        private readonly IDynamicTaskService _dynamicTaskService;
        private readonly TaskTimelineProcessor _taskTimelineProcessor;

        public TaskService(IFixedTaskService fixedTaskService, IDynamicTaskService dynamicTaskService)
        {
            _fixedTaskService = fixedTaskService;
            _dynamicTaskService = dynamicTaskService;
            _taskTimelineProcessor = new TaskTimelineProcessor();
        }

        public async Task<TasksForDayReturn> GetTasksForDay(DateTime date)
        {
            var returnData = new TasksForDayReturn()
            {
                Date = new DateOnly(date.Year, date.Month, date.Day)
            };

            var fixedTasks = _fixedTaskService.GetAll()
                                                .Where(ft => ft.StartTimestamp.Date == date.Date)
                                                .OrderBy(ft => ft.StartTimestamp)
                                                .AsEnumerable();

            var dynamicTasks = _dynamicTaskService.GetAll().AsEnumerable();

            return _taskTimelineProcessor.GetTasksForDay(fixedTasks, dynamicTasks, date);
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
                var tasksForDayReturn = new TasksForDayReturn()
                {
                    Date = new DateOnly(date.Year, date.Month, date.Day)
                };

                var fixedTasksForDay = fixedTasks.Where(ft => ft.StartTimestamp.Date == date.Date);
                yield return _taskTimelineProcessor.GetTasksForDay(fixedTasks, dynamicTasks, date);
            }
        }
    }
}
