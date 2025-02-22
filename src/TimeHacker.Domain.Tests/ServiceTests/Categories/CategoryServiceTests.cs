﻿using System.Drawing;
using FluentAssertions;
using Moq;
using TimeHacker.Domain.Contracts.Entities.Categories;
using TimeHacker.Domain.Contracts.Entities.ScheduleSnapshots;
using TimeHacker.Domain.Contracts.IRepositories.Categories;
using TimeHacker.Domain.Contracts.IServices.Categories;
using TimeHacker.Domain.Services.Categories;
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

        public CategoryServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _categoryService = new CategoryService(_categoriesRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new Category()
            {
                Name = "TestCategory1000",
                UserId = "IncorrectUserId"
            };
            await _categoryService.AddAsync(newEntry);
            var result = _categories.FirstOrDefault(x => x.Id == newEntry.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.UserId.Should().Be(userId);
        }

        [Fact]
        [Trait("UpdateAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new Category()
            {
                Id = _categories.First(x => x.UserId == userId).Id,
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
                SetupMocks(userId);

                var newEntry = new Category()
                {
                    Id = _categories.First(x => x.UserId != userId).Id,
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
            SetupMocks(userId);

            var idToDelete = _categories.First(x => x.UserId == userId).Id;
            await _categoryService.DeleteAsync(idToDelete);
            var result = _categories.FirstOrDefault(x => x.Id == idToDelete);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _categoryService.DeleteAsync(_categories.First(x => x.UserId != userId).Id);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = _categoryService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(_categories.Where(x => x.UserId == userId).ToList());
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return correct data")]
        public async Task GetByIdAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var id = _categories.First(x => x.UserId == userId).Id;
            var result = await _categoryService.GetByIdAsync(id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        [Trait("GetByIdAsync", "Should return nothing on incorrect userId")]
        public async Task GetByIdAsync_ShouldThrow()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = await _categoryService.GetByIdAsync(_categories.First(x => x.UserId != userId).Id);
            result.Should().BeNull();
        }

        [Fact]
        [Trait("UpdateScheduleEntityAsync", "Should update schedule entry")]
        public async Task UpdateScheduleEntityAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new ScheduleEntity();
            var categoryId = _categories.First(x => x.UserId == userId).Id;
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
                var userId = "TestIdentifier";
                SetupMocks(userId);

                var newEntry = new ScheduleEntity();
                await _categoryService.UpdateScheduleEntityAsync(newEntry, _categories.First(x => x.UserId != userId).Id);
            });
        }

        #region Mock helpers

        private void SetupMocks(string userId)
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
                    UserId = "IncorrectUserId",
                    Name = "TestCategory3",
                    Description = "Test description",
                    ScheduleEntity = new ScheduleEntity()
                },

                new()
                {
                    UserId = "IncorrectUserId",
                    Name = "TestCategory4",
                    Description = "Test description",
                }
            ];

            _categoriesRepository.As<IRepositoryBase<Category, Guid>>().SetupRepositoryMock(_categories);
        }

        #endregion
    }
}
