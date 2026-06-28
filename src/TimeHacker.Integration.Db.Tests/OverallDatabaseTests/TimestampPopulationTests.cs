using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Timehacker.Integration.Db.Tests;
using TimeHacker.Domain.Entities.Categories;
using TimeHacker.Domain.IRepositories.Categories;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// SaveChangesAsync auto-populates CreatedTimestamp on insert and UpdatedTimestamp on modify, driven
// by the ChangeTracker and the injected TimeProvider.
public class TimestampPopulationTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("SaveChanges", "CreatedTimestamp")]
    public async Task Insert_Should_PopulateCreatedTimestampOnly()
    {
        var repo = Resolve<ICategoryRepository>();
        var category = new Category { Name = "Created", Color = Color.Teal };

        await repo.AddAndSaveAsync(category, TestContext.Current.CancellationToken);

        var reloaded = await ReloadAsync(category.Id);
        reloaded.CreatedTimestamp.Should().NotBe(default);
        reloaded.UpdatedTimestamp.Should().BeNull();
    }

    [Fact]
    [Trait("SaveChanges", "UpdatedTimestamp")]
    public async Task Update_Should_PopulateUpdatedTimestampAndKeepCreated()
    {
        var repo = Resolve<ICategoryRepository>();
        var category = new Category { Name = "Original", Color = Color.Teal };
        await repo.AddAndSaveAsync(category, TestContext.Current.CancellationToken);

        // Read the persisted (microsecond-precision) created timestamp so the later equality isn't a
        // false negative against the finer-grained in-memory DateTime. No-tracking keeps `category`
        // attached for the update below.
        var createdTimestamp = await Db.Set<Category>()
            .Where(c => c.Id == category.Id)
            .Select(c => c.CreatedTimestamp)
            .FirstAsync(TestContext.Current.CancellationToken);

        await Task.Delay(10, TestContext.Current.CancellationToken);
        category.Name = "Updated";
        await repo.UpdateAndSaveAsync(category, TestContext.Current.CancellationToken);

        var reloaded = await ReloadAsync(category.Id);
        reloaded.Name.Should().Be("Updated");
        reloaded.CreatedTimestamp.Should().Be(createdTimestamp);
        reloaded.UpdatedTimestamp.Should().NotBeNull();
        reloaded.UpdatedTimestamp!.Value.Should().BeAfter(createdTimestamp);
    }

    private async Task<Category> ReloadAsync(Guid id)
    {
        Db.ChangeTracker.Clear();
        return await Db.Set<Category>().FirstAsync(x => x.Id == id, TestContext.Current.CancellationToken);
    }
}
