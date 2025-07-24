using AwesomeAssertions;
using Moq;
using System.Drawing;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.Entities.ScheduleSnapshots;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Domain.IServices.Categories;
using TimeHacker.Domain.Services.Services.Categories;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.Categories
{
    public class CategoryServiceTests
    {
        #region Mocks

        private readonly Mock<ICategoryRepository> _categoriesRepository = new();

        #endregion

        #region Properties & constructor

        private List<Category> _categories;

        private readonly ICategoryService _categoryService;
        private readonly Guid _userId = Guid.NewGuid();
        public CategoryServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock(_userId, true);
            SetupMocks(_userId);
            _categoryService = new CategoryService(_categoriesRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var newEntry = new Category()
            {
                Name = "TestCategory1000"
            };
            await _categoryService.AddAsync(newEntry);
            var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.UserId.Should().Be(_userId);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var newEntry = new Category()
            {
                Id = _categories.First(x => x.UserId == _userId).Id,
                Name = "TestCategory1000"
            };
            await _categoryService.UpdateAsync(newEntry);
            var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var newEntry = new Category()
                {
                    Id = _categories.First(x => x.UserId != _userId).Id,
                    Name = "TestCategory1000"
                };
                await _categoryService.UpdateAsync(newEntry);
                var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var idToDelete = _categories.First(x => x.UserId == _userId).Id;
            await _categoryService.DeleteAsync(idToDelete);
            var result = _categories.FirstOrDefault(x => x.Id == idToDelete);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                await _categoryService.DeleteAsync(_categories.First(x => x.UserId != _userId).Id);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var result = _categoryService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(_categories.Where(x => x.UserId == _userId).ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var id = _categories.First(x => x.UserId == _userId).Id;
            var result = await _categoryService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var result = await _categoryService.GetByIdAsync(_categories.First(x => x.UserId != _userId).Id);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var newEntry = new ScheduleEntity();
            var categoryId = _categories.First(x => x.UserId == _userId).Id;
            await _categoryService.UpdateScheduleEntityAsync(newEntry, categoryId);
            var result = _categories.FirstOrDefault(x => x.Id == categoryId);
            result.Should().NotBeNull();
            result!.ScheduleEntity.Should().NotBeNull();
            result!.ScheduleEntity!.Id.Should().Be(newEntry.Id);
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateScheduleEntityAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var newEntry = new ScheduleEntity();
                await _categoryService.UpdateScheduleEntityAsync(newEntry, _categories.First(x => x.UserId != _userId).Id);
            });
        }

        #region Mock helpers

        private void SetupMocks(Guid userId)
        {
            _categories =
            [
                new()
                {
                    UserId = userId,
                    Name = "TestCategory1",
                    Color = Color.AliceBlue,
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = userId,
                    Name = "TestCategory2",
                    Description = "Test description",
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestCategory3",
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = Guid.NewGuid(),
                    Name = "TestCategory4",
                    Description = "Test description",
                }
            ];

            _categoriesRepository.As<IUserScopedRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories);
        }

        #endregion
    }
}
