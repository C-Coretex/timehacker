using AwesomeAssertions;
using Moq;
using System.Drawing;
using System.Linq.Expressions;
using TimeHacker.Application.Api.AppServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.Models.EntityModels;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.ScheduleSnapshots
{
    public class ScheduleEntityAppServiceTests
    {        
        #region Mocks

        private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();
        private readonly Mock<IFixedTaskRepository> _fixedTasksRepository = new();
        private readonly Mock<ICategoryRepository> _categoriesRepository = new();

        #endregion

        #region Properties & constructor

        private List<FixedTask> _fixedTasks;
        private List<Category> _categories;
        private List<ScheduleEntity> _scheduledEntities;

        private readonly Guid _userId = Guid.NewGuid();

        private readonly IScheduleEntityAppService _scheduleEntityAppService;

        public ScheduleEntityAppServiceTests()
        {
            SetupMocks(_userId);
            _scheduleEntityAppService = new ScheduleEntityAppService(_scheduleEntityRepository.Object, _fixedTasksRepository.Object, _categoriesRepository.Object);
        }

        #endregion

        [Theory, CombinatorialData]
        [Trait("Save", "Should save data")]
        public async Task Save_ShouldSaveData(bool isCategory)
        {
            var repeatingEntity = new RepeatingEntityModel()
            {
                EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                RepeatingData = new DayRepeatingEntity(2)
            };

            var inputData = new InputScheduleEntityModel()
            {
                ScheduleEntityParentEnum =
                    isCategory ? ScheduleEntityParentEnum.Category : ScheduleEntityParentEnum.FixedTask,
                ParentEntityId = isCategory
                    ? _categories.First(x => x.UserId == _userId).Id
                    : _fixedTasks.First(x => x.UserId == _userId).Id,
                RepeatingEntityModel = repeatingEntity
            };
            var expected = await _scheduleEntityAppService.Save(inputData);

            var actual2 = isCategory
                ? _categories.First(x => x.Id == inputData.ParentEntityId).ScheduleEntityId
                : _fixedTasks.First(x => x.Id == inputData.ParentEntityId).ScheduleEntityId;

            expected.Id.Should().Be(actual2!.Value);
            var scheduledEntity = _scheduledEntities.First(x => x.Id == expected.Id);
            scheduledEntity.Should().NotBeNull();
            scheduledEntity.RepeatingEntity.Should().Be(repeatingEntity);
        }


        [Theory, CombinatorialData]
        [Trait("Save", "Should throw exception on incorrect data")]
        public async Task Save_ShouldThrow(bool existingEntry, bool isCategory)
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var actual = await _scheduleEntityAppService.Save(new InputScheduleEntityModel()
                {
                    ScheduleEntityParentEnum = isCategory ? ScheduleEntityParentEnum.Category : ScheduleEntityParentEnum.FixedTask,
                    ParentEntityId = existingEntry ? _scheduledEntities.First(x => x.UserId != _userId).Id : Guid.NewGuid(),
                    RepeatingEntityModel = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                        RepeatingData = new DayRepeatingEntity(1)
                    }
                });
            });
        }


        #region Mock helpers

        private void SetupMocks(Guid userId)
        {
            _scheduledEntities =
            [
                new()
                {
                    UserId = userId,
                    CreatedTimestamp = DateTime.Now,
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                        RepeatingData = new DayRepeatingEntity(2)
                    },
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },

                new()
                {
                    UserId = userId,
                    CreatedTimestamp = DateTime.Now,
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    CreatedTimestamp = DateTime.Now,
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }]
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    CreatedTimestamp = DateTime.Now,
                }
            ];

            _scheduleEntityRepository.As<IUserScopedRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduledEntities);


            _fixedTasks =
            [
                new()
                {
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(1),
                    EndTimestamp = DateTime.Now.AddHours(1).AddMinutes(30),
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(2),
                    EndTimestamp = DateTime.Now.AddHours(2).AddMinutes(30)
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask3",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30),
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
                }
            ];

            _fixedTasksRepository.As<IUserScopedRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);

            _fixedTasksRepository.Setup(x => x.UpdateProperty(It.IsAny<Expression<Func<FixedTask, bool>>>(), It.IsAny<Func<FixedTask, Guid?>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .Callback<Expression<Func<FixedTask, bool>>, Func<FixedTask, Guid?>, Guid?, CancellationToken>((predicate, _, value, _) =>
                {
                    var items = _fixedTasks.Where(predicate.Compile());
                    foreach (var item in items)
                        item.ScheduleEntityId = value;
                });

            _categories =
            [
                new()
                {
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Color = Color.AliceBlue,
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Description = "Test description",
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask3",
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask4",
                    Description = "Test description",
                }
            ];

            _categoriesRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories);
            _categoriesRepository.Setup(x => x.UpdateProperty(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<Func<Category, Guid?>>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .Callback<Expression<Func<Category, bool>>, Func<Category, Guid?>, Guid?, CancellationToken>((predicate, _, value, _) =>
                {
                    var items = _categories.Where(predicate.Compile());
                    foreach (var item in items)
                        item.ScheduleEntityId = value;
                });
        }

        #endregion
    }
}
