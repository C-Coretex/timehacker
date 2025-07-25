using Moq;
using TimeHacker.Application.Api.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Application.Api.Services.ScheduleSnapshots;
using TimeHacker.Application.Api.Tests.Mocks;
using TimeHacker.Application.Api.Tests.Mocks.Extensions;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.ScheduleSnapshots;

namespace TimeHacker.Application.Api.Tests.ServiceTests.ScheduleSnapshots
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

            _scheduledCategoryRepository.As<IUserScopedRepositoryBase<ScheduledCategory, Guid>>().SetupRepositoryMock(_scheduledCategories);
        }

        #endregion
    }
}
