using Moq;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.Services.ScheduleSnapshots;
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

        private readonly Guid _userId = Guid.NewGuid();

        public ScheduledCategoryServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock(_userId, true);

            _scheduledCategoryService = new ScheduledCategoryService(_scheduledCategoryRepository.Object, userAccessor);
        }

        #endregion



        #region Mock helpers

        private void SetupMocks(Guid userId)
        {
            _scheduledCategories =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "TestFixedTask1",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "TestFixedTask2",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask3",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "TestFixedTask4",
                    Date =  DateOnly.FromDateTime(DateTime.Now),
                    Description = "Test description",
                }
            ];

            _scheduledCategoryRepository.As<IRepositoryBase<ScheduledCategory, Guid>>().SetupRepositoryMock(_scheduledCategories);
        }

        #endregion
    }
}
