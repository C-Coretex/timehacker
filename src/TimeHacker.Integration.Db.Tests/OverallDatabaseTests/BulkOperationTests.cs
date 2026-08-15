namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

// DeleteBy/UpdateProperty run as ExecuteDelete/ExecuteUpdate SQL: they bypass the change tracker, stay
// user-scoped, and (for UpdateProperty) still stamp UpdatedTimestamp for IUpdatable entities.
public class BulkOperationTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("DeleteBy", "Scoped+Predicate")]
    public async Task DeleteBy_Should_RemoveOnlyMatchingCurrentUserRows()
    {
        var repo = Resolve<ICategoryRepository>();
        await repo.AddAndSaveAsync(new Category { Name = "del", Color = Color.Red }, TestContext.Current.CancellationToken);
        await repo.AddAndSaveAsync(new Category { Name = "del", Color = Color.Red }, TestContext.Current.CancellationToken);
        await repo.AddAndSaveAsync(new Category { Name = "keep", Color = Color.Red }, TestContext.Current.CancellationToken);

        var otherUser = OtherUsers.First();
        await otherUser.Resolve<ICategoryRepository>().AddAndSaveAsync(new Category { Name = "del", Color = Color.Red }, TestContext.Current.CancellationToken);

        var deletedCount = await repo.DeleteBy(c => c.Name == "del", TestContext.Current.CancellationToken);

        deletedCount.Should().Be(2);

        Db.ChangeTracker.Clear();
        var remaining = await Db.Set<Category>().ToListAsync(TestContext.Current.CancellationToken);
        remaining.Should().HaveCount(2);
        remaining.Should().ContainSingle(c => c.UserId == CurrentUser.UserId && c.Name == "keep");
        remaining.Should().ContainSingle(c => c.UserId == otherUser.UserId && c.Name == "del");
    }

    [Fact]
    [Trait("UpdateProperty", "Column+UpdatedTimestamp")]
    public async Task UpdateProperty_Should_SetColumnAndUpdatedTimestamp()
    {
        var repo = Resolve<ICategoryRepository>();
        var category = new Category { Name = "Original", Color = Color.Red };
        await repo.AddAndSaveAsync(category, TestContext.Current.CancellationToken);
        category.UpdatedTimestamp.Should().BeNull();

        await repo.UpdateProperty(c => c.Id == category.Id, c => c.Name, "Renamed", TestContext.Current.CancellationToken);

        Db.ChangeTracker.Clear();
        var reloaded = await Db.Set<Category>().FirstAsync(c => c.Id == category.Id, TestContext.Current.CancellationToken);
        reloaded.Name.Should().Be("Renamed");
        reloaded.UpdatedTimestamp.Should().NotBeNull();
    }
}
