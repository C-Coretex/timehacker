using System.Drawing;
using AutoMapper;
using FluentAssertions;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Models.EntityModels;
using TimeHacker.Domain.Contracts.Models.EntityModels.Enums;
using TimeHacker.Domain.Contracts.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Contracts.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Domain.Tests.Helpers;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduleEntityServiceTests
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

        private readonly IScheduleEntityService _scheduleEntityService;

        public ScheduleEntityServiceTests()
        {
            var userAccessor = new UserAccessorMock("TestIdentifier", true);
            var mapperConfiguration = AutomapperHelpers.GetMapperConfiguration();
            var mapper = new Mapper(mapperConfiguration);

            var fixedTaskService = new FixedTaskService(_fixedTasksRepository.Object, userAccessor);
            var categoryService = new CategoryService(_categoriesRepository.Object, userAccessor);
            _scheduleEntityService = new ScheduleEntityService(_scheduleEntityRepository.Object, fixedTaskService, categoryService, userAccessor, mapper);
        }

        #endregion
       
        [Fact]
        [Trait("GetAllFrom", "Should return correct data")]
        public void GetAllFrom_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            var date = DateTime.Now;
            SetupMocks(userId);

            _scheduledEntities.Clear();
            _scheduledEntities.AddRange(
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()],
                    EndsOn = null
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
                    Id = 3,
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-1).AddMinutes(10),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(10))
                },

                new()
                {
                    Id = 4,
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-2),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-2))
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()],
                    EndsOn = null
                },

                new()
                {
                    Id = 5,
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
                    Id = 6,
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = date.AddDays(-2),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-2))
                }
            ]);

            var from = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(-1));
            var actual = _scheduleEntityService.GetAllFrom(from).ToList();
            actual.Should().NotBeNull();

            var expected = _scheduledEntities.Where(x => x.UserId == userId && (x.EndsOn == null || x.EndsOn >= from))
                .ToList();
            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("UpdateLastEntityCreated", "Should update data")]
        public async Task UpdateLastEntityCreated_ShouldUpdateData()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var lastEntryCreated = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
            await _scheduleEntityService.UpdateLastEntityCreated(1, lastEntryCreated);
            var actual = _scheduledEntities.First(x => x.Id == 1);
            actual.LastEntityCreated.Should().Be(lastEntryCreated);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect data")]
        public async Task UpdateLastEntityCreated_ShouldThrow(bool existingEntry)
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _scheduleEntityService.UpdateLastEntityCreated(existingEntry ? (uint)3: 100, DateOnly.FromDateTime(DateTime.Now));
            });
        }


        [Fact]
        [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect entity created data")]
        public async Task UpdateLastEntityCreated_ShouldThrowOnIncorrectEntityCreated()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _scheduleEntityService.UpdateLastEntityCreated(1, DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
            });
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [Trait("Save", "Should save data")]
        public async Task Save_ShouldSaveData(bool isCategory)
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var repeatingEntity = new RepeatingEntityModel()
            {
                EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                RepeatingData = new DayRepeatingEntity(2)
            };
            var expected = await _scheduleEntityService.Save(new InputScheduleEntityModel()
            {
                ScheduleEntityParentEnum = isCategory ? ScheduleEntityParentEnum.Category : ScheduleEntityParentEnum.FixedTask,
                ParentEntityId = 1,
                RepeatingEntityModel = repeatingEntity
            });

            var actual2 = isCategory
                ? _categories.First(x => x.Id == 1).ScheduleEntity
                : _fixedTasks.First(x => x.Id == 1).ScheduleEntity;

            expected.Should().Be(actual2);
            actual2!.RepeatingEntity.Should().Be(repeatingEntity);
            actual2!.UserId.Should().Be(userId);
        }


        [Theory]
        [MemberData(nameof(TheoryDataHelpers.TwoBoolPermutationData), MemberType = typeof(TheoryDataHelpers))]
        [Trait("Save", "Should throw exception on incorrect data")]
        public async Task Save_ShouldThrow(bool existingEntry, bool isCategory)
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                var actual = await _scheduleEntityService.Save(new InputScheduleEntityModel()
                {
                    ScheduleEntityParentEnum = isCategory ? ScheduleEntityParentEnum.Category : ScheduleEntityParentEnum.FixedTask,
                    ParentEntityId = existingEntry ? (uint)3 : 100,
                    RepeatingEntityModel = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                        RepeatingData = new DayRepeatingEntity(1)
                    }
                });
            });
        }


        #region Mock helpers

        private void SetupMocks(string userId)
        {
            _scheduledEntities =
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    CreatedTimestamp = DateTime.Now,
                    RepeatingEntity = new RepeatingEntityModel()
                    {
                        EntityType = RepeatingEntityTypeEnum.DayRepeatingEntity,
                        RepeatingData = new DayRepeatingEntity(2)
                    }, 
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    CreatedTimestamp = DateTime.Now,
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = DateTime.Now,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = DateTime.Now,
                }
            ];

            _scheduleEntityRepository.As<IRepositoryBase<ScheduleEntity, uint>>().SetupRepositoryMock(_scheduledEntities);


            _fixedTasks =
            [
                new()
                {
                    Id = 1,
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
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(2),
                    EndTimestamp = DateTime.Now.AddHours(2).AddMinutes(30)
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30),
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
                }
            ];

            _fixedTasksRepository.As<IRepositoryBase<FixedTask, uint>>().SetupRepositoryMock(_fixedTasks);


            _categories =
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Color = Color.AliceBlue,
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Description = "Test description",
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Description = "Test description",
                }
            ];

            _categoriesRepository.As<IRepositoryBase<Category, uint>>().SetupRepositoryMock(_categories);
        }

        #endregion
    }
}

