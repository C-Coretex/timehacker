using Moq;
using System.Drawing;
using AwesomeAssertions;
using TimeHacker.Domain.Contracts.Entities.Tags;
using TimeHacker.Domain.Contracts.IRepositories.Tags;
using TimeHacker.Domain.Contracts.IServices.Tags;
using TimeHacker.Domain.Services.Tags;
using TimeHacker.Domain.Tests.Mocks;
using TimeHacker.Domain.Tests.Mocks.Extensions;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

namespace TimeHacker.Domain.Tests.ServiceTests.Tags
{
    public class TagServiceTests
    {
        #region Mocks

        private readonly Mock<ITagRepository> _tagRepository = new();

        #endregion

        #region Properties & constructor

        private List<Tag> _tags;

        private readonly ITagService _tagService;

        public TagServiceTests()
        {
            var userAccessor = new UserAccessorBaseMock("TestIdentifier", true);

            _tagService = new TagService(_tagRepository.Object, userAccessor);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new Tag()
            {
                Id = Guid.NewGuid(),
                Name = "TestTag1000",
                UserId = "IncorrectUserId"
            };
            await _tagService.AddAsync(newEntry);
            var result = _tags.FirstOrDefault(x => x.Id == newEntry.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
            result!.UserId.Should().Be(userId);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var newEntry = new Tag()
            {
                Id = _tags.First(x => x.UserId == userId).Id,
                Name = "TestTag1000"
            };
            await _tagService.UpdateAsync(newEntry);
            var result = _tags.FirstOrDefault(x => x.Id == newEntry.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task UpdateAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                var newEntry = new Tag()
                {
                    Id = _tags.First(x => x.UserId != userId).Id,
                    Name = "TestTag1000"
                };
                await _tagService.UpdateAsync(newEntry);
                var result = _tags.FirstOrDefault(x => x.Id == newEntry.Id);
            });
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var idToDelete = _tags.First(x => x.UserId == userId).Id;
            await _tagService.DeleteAsync(idToDelete);
            var result = _tags.FirstOrDefault(x => x.Id == idToDelete);

            result.Should().BeNull();
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should throw exception on incorrect userId")]
        public async Task DeleteAsync_ShouldThrow()
        {
            await Assert.ThrowsAnyAsync<Exception>(async () =>
            {
                var userId = "TestIdentifier";
                SetupMocks(userId);

                await _tagService.DeleteAsync(_tags.First(x => x.UserId != userId).Id);
            });
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public void GetAll_ShouldReturnCorrectData()
        {
            var userId = "TestIdentifier";
            SetupMocks(userId);

            var result = _tagService.GetAll().ToList();

            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(_tags.Where(x => x.UserId == userId));
        }

        #region Mock helpers

        private void SetupMocks(string userId)
        {
            _tags =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "TestTag1",
                    Color = Color.AliceBlue,
                    Category = "TestCategory"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Name = "TestTag2",
                    Color = Color.AliceBlue,
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = "IncorrectUserId",
                    Name = "TestTag3",
                    Color = Color.AliceBlue,
                    Category = "TestCategory"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = "IncorrectUserId",
                    Name = "TestTag4",
                    Color = Color.AliceBlue,
                }
            ];

            _tagRepository.As<IRepositoryBase<Tag, Guid>>().SetupRepositoryMock(_tags);
        }

        #endregion
    }
}
