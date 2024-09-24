using System.Drawing;
using AutoMapper;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Tasks;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Services.Tasks;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Tests.Helpers;
using TimeHacker.Tests.Mocks;
using TimeHacker.Tests.Mocks.Extensions;

namespace TimeHacker.Tests.ServiceTests.ScheduleSnapshots
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
       /* 
        [Fact]
        [Trait("GetBy", "Should return correct data")]
        public async Entity DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupFixeEntityMocks(userId);

            var result = await _scheduleEntityService.GetBy(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetBy", "Should throw exception on incorrect userId")]
        public async Entity DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupFixeEntityMocks(userId);

                var result = await _scheduleEntityService.GetBy(3);
            });
        }*/

        #region Mock helpers

        private void SetupFixeEntityMocks(string userId)
        {
            _scheduledEntities =
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    ScheduledTasks = [new ScheduledTask()],
                    ScheduledCategories = [new ScheduledCategory()]
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
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

