using System.Drawing;
using AwesomeAssertions;
using Moq;
using TimeHacker.Application.Api.AppServices.Tags;
using TimeHacker.Application.Api.Contracts.IAppServices.Tags;
using TimeHacker.Domain.Entities.Tags;
using TimeHacker.Domain.IRepositories;
using TimeHacker.Domain.IRepositories.Tags;
using TimeHacker.Helpers.Tests.Mocks.Extensions;

namespace TimeHacker.Application.Api.Tests.AppServiceTests.Tags
{
    public class TagServiceTests
    {
        #region Mocks

        private readonly Mock<ITagRepository> _tagRepository = new();

        #endregion

        #region Properties & constructor

        private List<Tag> _tags;

        private readonly ITagAppService _tagService;
        private readonly Guid _userId = Guid.NewGuid();

        public TagServiceTests()
        {
            SetupMocks(_userId);
            _tagService = new TagService(_tagRepository.Object);
        }

        #endregion

        [Fact]
        [Trait("AddAndSaveAsync", "Should add entry with correct userId")]
        public async Task AddAsync_ShouldAddEntry()
        {
            var newEntry = new Tag()
            {
                Id = Guid.NewGuid(),
                Name = "TestTag1000",
                UserId = Guid.NewGuid()
            };
            await _tagService.AddAsync(newEntry);
            var result = _tags.FirstOrDefault(x => x.Id == newEntry.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("UpdateAndSaveAsync", "Should update entry")]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var newEntry = new Tag()
            {
                Id = _tags.First(x => x.UserId == _userId).Id,
                Name = "TestTag1000"
            };
            await _tagService.UpdateAsync(newEntry);
            var result = _tags.FirstOrDefault(x => x.Id == newEntry.Id);

            result.Should().NotBeNull();
            result!.Name.Should().Be(newEntry.Name);
        }

        [Fact]
        [Trait("DeleteAndSaveAsync", "Should delete entry")]
        public async Task DeleteAsync_ShouldUpdateEntry()
        {
            var idToDelete = _tags.First(x => x.UserId == _userId).Id;
            await _tagService.DeleteAsync(idToDelete);
            var result = _tags.FirstOrDefault(x => x.Id == idToDelete);

            result.Should().BeNull();
        }

        [Fact]
        [Trait("GetAll", "Should return correct data")]
        public async Task GetAll_ShouldReturnCorrectData()
        {
            var result = await _tagService.GetAll().ToListAsync();

            result.Count.Should().Be(_tags.Count);
            result.Should().BeEquivalentTo(_tags);
        }

        #region Mock helpers

        private void SetupMocks(Guid userId)
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
                    UserId = Guid.NewGuid(),
                    Name = "TestTag3",
                    Color = Color.AliceBlue,
                    Category = "TestCategory"
                },

                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Name = "TestTag4",
                    Color = Color.AliceBlue,
                }
            ];

            _tagRepository.As<IUserScopedRepositoryBase<Tag, Guid>>().SetupRepositoryMock(_tags);
        }

        #endregion
    }
}
