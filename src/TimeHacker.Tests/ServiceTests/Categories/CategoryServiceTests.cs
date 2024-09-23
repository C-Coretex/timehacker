﻿using System.Drawing;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IModels;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Services.Categories;
using TimeHacker.Helpers.Domain.Abstractions.Delegates;
using TimeHacker.Tests.Mocks;

namespace TimeHacker.Tests.ServiceTests.Categories
{
    public class CategoryServiceTests
    {
        #region Mocks

        private readonly Mock<ICategoryRepository> _categoriesRepository = new();

        private readonly IUserAccessor userAccessor;

        #endregion

        #region Properties & constructor

        private List<Category> _categories;

        private readonly ICategoryService _categoryService;

        public CategoryServiceTests()
        {
            var userAccessor = new UserAccessorMock("TestIdentifier", true);

            _categoryService = new CategoryService(_categoriesRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var newEntry = new Category()
            {
                Id = 1000,
                Name = "TestCategory1000"
            };
            await _categoryService.AddAsync(newEntry);
            var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var newEntry = new Category()
            {
                Id = 1,
                Name = "TestCategory1000"
            };
            await _categoryService.UpdateAsync(newEntry);
            var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupCategoryMocks(userId);

                var newEntry = new Category()
                {
                    Id = 3,
                    Name = "TestCategory1000"
                };
                await _categoryService.UpdateAsync(newEntry);
                var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            await _categoryService.DeleteAsync(1);
            var result = _categories.FirstOrDefault(x => x.Id == 1);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupCategoryMocks(userId);

                await _categoryService.DeleteAsync(3);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var result = _categoryService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Select(x => x.Id).Should().BeEquivalentTo([1, 2]);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var result = await _categoryService.GetByIdAsync(1);
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var result = await _categoryService.GetByIdAsync(3);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupCategoryMocks(userId);

            var newEntry = new ScheduleEntity()
            {
                Id = 100
            };
            await _categoryService.UpdateScheduleEntityAsync(newEntry, 1);
            var result = _categories.FirstOrDefault(x => x.Id == 1);
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
                var userId = "TestIdentifier";
                SetupCategoryMocks(userId);

                var newEntry = new ScheduleEntity()
                {
                    Id = 1
                };
                await _categoryService.UpdateScheduleEntityAsync(newEntry, 3);
            });
        }

        #region Mock helpers

        private void SetupCategoryMocks(string userId)
        {
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

            _categoriesRepository.Setup(x => x.AddAsync(It.IsAny<Category>(), It.IsAny<bool>()))
                .Callback<Category, bool>((entry, _) => _categories.Add(entry));

            _categoriesRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>(), It.IsAny<bool>()))
                .Callback<Category, bool>((entry, _) =>
                {
                    _categories.RemoveAll(x => x.Id == entry.Id);
                    _categories.Add(entry);
                })
                .Returns<Category, bool>((entry, _) => Task.FromResult(entry));

            _categoriesRepository.Setup(x => x.GetByIdAsync(It.IsAny<uint>(), It.IsAny<bool>(), It.IsAny<IncludeExpansionDelegate<Category>[]>()))
                .Returns<uint, bool, IncludeExpansionDelegate<Category>[]>((id, _, _) => Task.FromResult(_categories.FirstOrDefault(x => x.Id == id)));

            _categoriesRepository.Setup(x => x.DeleteAsync(It.IsAny<Category>(), It.IsAny<bool>()))
                .Callback<Category, bool>((entry, _) => _categories.RemoveAll(x => x.Id == entry.Id));

            _categoriesRepository.Setup(x => x.GetAll(It.IsAny<bool>()))
                .Returns(_categories.AsQueryable().BuildMock());
        }

        #endregion
    }
}