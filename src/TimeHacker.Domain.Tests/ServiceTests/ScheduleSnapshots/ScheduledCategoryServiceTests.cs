using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.ScheduleSnapshots
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
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _scheduledCategoryService = new ScheduledCategoryService(_scheduledCategoryRepository.Object, userAccessor);
        }

        #endregion



        #region Mock helpers

        private void SetupMocks(string userId)
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
