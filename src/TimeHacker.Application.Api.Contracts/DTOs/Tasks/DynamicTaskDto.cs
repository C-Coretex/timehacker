using System.Linq.Expressions;
using TimeHacker.Application.Api.Contracts.DTOs.Tags;
using TimeHacker.Domain.Entities.Tasks;

namespace TimeHacker.Application.Api.Contracts.DTOs.Tasks
{
    public record DynamicTaskDto()
    {
        public Guid? Id { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public byte Priority { get; init; }
        public TimeSpan MinTimeToFinish { get; init; }
        public TimeSpan MaxTimeToFinish { get; init; }
        public TimeSpan? OptimalTimeToFinish { get; init; }
        public DateTime CreatedTimestamp { get; init; }
        public IEnumerable<TagDto> Tags { get; init; } = [];

        public static Expression<Func<DynamicTask, DynamicTaskDto>> Selector =>
            x => new DynamicTaskDto()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Priority = x.Priority,
                MinTimeToFinish = x.MinTimeToFinish,
                MaxTimeToFinish = x.MaxTimeToFinish,
                OptimalTimeToFinish = x.OptimalTimeToFinish,
                CreatedTimestamp = x.CreatedTimestamp,
                Tags = x.TagDynamicTasks.Select(tagTask => TagDto.Create(tagTask.Tag))
            };

        private static readonly Func<DynamicTask, DynamicTaskDto> CreateFunc = Selector.Compile();
        public static DynamicTaskDto? Create(DynamicTask? entity) => entity != null ? CreateFunc(entity) : null;

        public DynamicTask GetEntity(DynamicTask? entity = null)
        {
            entity ??= new DynamicTask();

            entity.Name = Name;
            entity.Description = Description;
            entity.Priority = Priority;
            entity.MinTimeToFinish = MinTimeToFinish;
            entity.MaxTimeToFinish = MaxTimeToFinish;
            entity.OptimalTimeToFinish = OptimalTimeToFinish;

            return entity;
        }
    }
}
