using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.Entities.Tasks;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;
using TimeHacker.Tests.Mocks;
using TimeHacker.Tests.Mocks.Extensions;

namespace TimeHacker.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduledCategoryServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledCategoryRepository> _scheduledCategoryRepository = new();

        #endregion

        #region Properties & constructor

        private List<ScheduledCategory> _scheduledCategories;

        private readonly IScheduledCategoryService _scheduledCategoryService;

        public ScheduledCategoryServiceTests()
        {
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _scheduledCategoryService = new ScheduledCategoryService(_scheduledCategoryRepository.Object, userAccessor);
        }

        #endregion



        #region Mock helpers

        private void SetupFixedTaskMocks(string userId)
        {
            _scheduledCategories =
            [
                new()
                {
                    Id = 1,
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 2,
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                },

                new()
                {
                    Id = 3,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask3",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = 4,
                    UserId = "IncorrectUserId",
                    Name = "TestFixedTask4",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                }
            ];

            _scheduledCategoryRepository.As<IRepositoryBase<ScheduledCategory, ulong>>().SetupRepositoryMock(_scheduledCategories);
        }

        #endregion
    }
}
