﻿using System.Drawing;
using System.Linq.Expressions;
using AwesomeAssertions;
using Moq;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.Tasks;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.Tasks;
using TimeHacker.Domain.IServices;
using TimeHacker.Domain.Models.EntityModels;
using TimeHacker.Domain.Models.EntityModels.Enums;
using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
using TimeHacker.Domain.Services.Services;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Domain.Services.Tests.ServiceTests
{
    public class ScheduleEntityServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduleEntityRepository> _scheduleEntityRepository = new();

        #endregion

        #region Properties & constructor

        private List<FixedTask> _fixedTasks;
        private List<Category> _categories;
        private List<ScheduleEntity> _scheduledEntities;

        private readonly Guid _userId = Guid.NewGuid();

        private readonly IScheduleEntityService _scheduleEntityService;

        public ScheduleEntityServiceTests()
        {
            SetupMocks(_userId);
            _scheduleEntityService = new ScheduleEntityService(_scheduleEntityRepository.Object);
        }

        #endregion
       
        [Fact]
        [Trait("GetAllFrom", "Should return correct data")]
        public void GetAllFrom_ShouldReturnCorrectData()
        {
            var date = DateTime.Now;

            _scheduledEntities.Clear();
            _scheduledEntities.AddRange(
            [
                new()
                {
                    UserId = _userId,
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }],
                    EndsOn = null
                },

                new()
                {
                    UserId = _userId,
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
                    UserId = _userId,
                    CreatedTimestamp = date.AddDays(-1).AddMinutes(10),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(10))
                },

                new()
                {
                    UserId = _userId,
                    CreatedTimestamp = date.AddDays(-2),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-2))
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    CreatedTimestamp = date,
                    ScheduledTasks = [new ScheduledTask() { Name = "" }],
                    ScheduledCategories = [new ScheduledCategory() { Name = "" }],
                    EndsOn = null
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    CreatedTimestamp = date.AddDays(-1),
                    EndsOn = DateOnly.FromDateTime(date)
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    CreatedTimestamp = date.AddDays(-2),
                    EndsOn = DateOnly.FromDateTime(date.AddDays(-2))
                }
            ]);

            var from = DateOnly.FromDateTime(date.AddDays(-1).AddMinutes(-1));
            var actual = _scheduleEntityService.GetAllFrom(from).ToList();
            actual.Should().NotBeNull();

            var expected = _scheduledEntities.Where(x => x.EndsOn == null || x.EndsOn >= from)
                .ToList();
            actual.Count.Should().Be(expected.Count);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        [Trait("UpdateLastEntityCreated", "Should update data")]
        public async Task UpdateLastEntityCreated_ShouldUpdateData()
        {
            var lastEntryCreated = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
            var scheduleEntityId = _scheduledEntities.First(x => x.UserId == _userId).Id;
            await _scheduleEntityService.UpdateLastEntityCreated(scheduleEntityId, lastEntryCreated);
            var actual = _scheduledEntities.First(x => x.Id == scheduleEntityId);
            actual.LastEntityCreated.Should().Be(lastEntryCreated);
        }

        [Theory, CombinatorialData]
        [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect data")]
        public async Task UpdateLastEntityCreated_ShouldThrow(bool existingEntry)
        {
            if (!existingEntry)
            {
                await _scheduleEntityService.UpdateLastEntityCreated(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now));
                return;
            }

            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await _scheduleEntityService.UpdateLastEntityCreated(_scheduledEntities.First(x => x.UserId != _userId).Id, DateOnly.FromDateTime(DateTime.Now));
            });
        }


        [Fact]
        [Trait("UpdateLastEntityCreated", "Should throw exception on incorrect entity created data")]
        public async Task UpdateLastEntityCreated_ShouldThrowOnIncorrectEntityCreated()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await _scheduleEntityService.UpdateLastEntityCreated(
                    _scheduledEntities.First(x => x.UserId == _userId).Id, 
                    DateOnly.FromDateTime(DateTime.Now.AddDays(1)));
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
        }

        #endregion
    }
}

