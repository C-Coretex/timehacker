﻿using System.Drawing;
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
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);
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
                    UserId = userId,
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()],
                    EndsOn = null
                },

                new()
                {
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-1).AddMinutes(10),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(10))
                },

                new()
                {
                    UserId = userId,
                    CreatedTimestamp = date.AddDays(-2),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-2))
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()],
                    EndsOn = null
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
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
            var scheduleEntityId = _scheduledEntities.First(x => x.UserId == userId).Id;
            await _scheduleEntityService.UpdateLastEntityCreated(scheduleEntityId, lastEntryCreated);
            var actual = _scheduledEntities.First(x => x.Id == scheduleEntityId);
            actual.LastEntityCreated.Should().Be(lastEntryCreated);
        }

        [Theory, CombinatorialData]
        [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect data")]
        public async Task UpdateLastEntityCreated_ShouldThrow(bool existingEntry)
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _scheduleEntityService.UpdateLastEntityCreated(existingEntry ? _scheduledEntities.First(x => x.UserId != userId).Id : Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now));
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

                await _scheduleEntityService.UpdateLastEntityCreated(_scheduledEntities.First(x => x.UserId == userId).Id, DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
            });
        }

        [Theory, CombinatorialData]
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

            var inputData = new InputScheduleEntityModel()
            {
                ScheduleEntityParentEnum =
                    isCategory ? ScheduleEntityParentEnum.Category : ScheduleEntityParentEnum.FixedTask,
                ParentEntityId = isCategory
                    ? _categories.First(x => x.UserId == userId).Id
                    : _fixedTasks.First(x => x.UserId == userId).Id,
                RepeatingEntityModel = repeatingEntity
            };
            var expected = await _scheduleEntityService.Save(inputData);

            var actual2 = isCategory
                ? _categories.First(x => x.Id == inputData.ParentEntityId).ScheduleEntity
                : _fixedTasks.First(x => x.Id == inputData.ParentEntityId).ScheduleEntity;

            expected.Should().Be(actual2);
            actual2!.RepeatingEntity.Should().Be(repeatingEntity);
            actual2!.UserId.Should().Be(userId);
        }


        [Theory, CombinatorialData]
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
                    ParentEntityId = existingEntry ? _scheduledEntities.First(x => x.UserId != userId).Id : Guid.NewGuid(),
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
                    UserId = userId,
                    CreatedTimestamp = DateTime.Now,
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = DateTime.Now,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    CreatedTimestamp = DateTime.Now,
                }
            ];

            _scheduleEntityRepository.As<IRepositoryBase<ScheduleEntity, Guid>>().SetupRepositoryMock(_scheduledEntities);


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
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Priority = 1,
                    Description = "Test description",
                    StartTimestamp = DateTime.Now.AddDays(-2).AddHours(3),
                    EndTimestamp = DateTime.Now.AddHours(3).AddMinutes(30)
                }
            ];

            _fixedTasksRepository.As<IRepositoryBase<FixedTask, Guid>>().SetupRepositoryMock(_fixedTasks);


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
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Description = "Test description",
                }
            ];

            _categoriesRepository.As<IRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories);
        }

        #endregion
    }
}

