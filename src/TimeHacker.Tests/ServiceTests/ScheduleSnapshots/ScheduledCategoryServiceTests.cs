using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IServices.ScheduleSnapshots;
using TimeHacker.Domain.Services.ScheduleSnapshots;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Tests.Mocks;

namespace TimeHacker.Tests.ServiceTests.ScheduleSnapshots
{
    public class ScheduledCategoryServiceTests
    {
        #region Mocks

        private readonly Mock<IScheduledCategoryRepository> _scheduledCategoryRepository = new();

        private readonly IUserAccessor userAccessor;

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

            _scheduledCategoryRepository.Setup(x => x.AddAsync(It.IsAny<ScheduledCategory>(), It.IsAny<bool>()))
                .Callback<ScheduledCategory, bool>((entry, _) => _scheduledCategories.Add(entry));

            _scheduledCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<ScheduledCategory>(), It.IsAny<bool>()))
                .Callback<ScheduledCategory, bool>((entry, _) =>
                {
                    _scheduledCategories.RemoveAll(x => x.Id == entry.Id);
                    _scheduledCategories.Add(entry);
                })
                .Returns<ScheduledCategory, bool>((entry, _) => Task.FromResult(entry));

            _scheduledCategoryRepository.Setup(x => x.GetByIdAsync(It.IsAny<uint>(), It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<ScheduledCategory>[]>()))
                .Returns<uint, bool, IncludeExpansionDelegate<ScheduledCategory>[]>((id, _, _) => Task.FromResult(_scheduledCategories.FirstOrDefault(x => x.Id == id)));

            _scheduledCategoryRepository.Setup(x => x.DeleteAsync(It.IsAny<ScheduledCategory>(), It.IsAny<bool>()))
                .Callback<ScheduledCategory, bool>((entry, _) => _scheduledCategories.RemoveAll(x => x.Id == entry.Id));

            _scheduledCategoryRepository.Setup(x => x.GetAll(It.IsAny<bool>()))
                .Returns(_scheduledCategories.AsQueryable().BuildMock());
        }

        #endregion
    }
}
