using TimeHacker.Domain.IRepositories.Tags;
using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

public class ValueConverterTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
    [Fact]
    [Trait("DateTimeUtcConverter", "RoundTrip")]
    public async Task DateTime_Should_BeNormalizedToUtcOnRoundTrip()
    {
        // A non-UTC kind that the converter must normalize to UTC on the way in.
        var localStart = new DateTime(2026, 6, 1, 9, 30, 0, DateTimeKind.Local);
        var localEnd = new DateTime(2026, 6, 1, 10, 30, 0, DateTimeKind.Local);

        var task = new FixedTask
        {
            Name = "UTC round-trip",
            Priority = 1,
            StartTimestamp = localStart,
            EndTimestamp = localEnd
        };
        await Resolve<IFixedTaskRepository>().AddAndSaveAsync(task, TestContext.Current.CancellationToken);

        var reloaded = await ReloadAsync<FixedTask>(task.Id);

        reloaded.StartTimestamp.Kind.Should().Be(DateTimeKind.Utc);
        reloaded.EndTimestamp.Kind.Should().Be(DateTimeKind.Utc);
        reloaded.StartTimestamp.Should().Be(localStart.ToUniversalTime());
        reloaded.EndTimestamp.Should().Be(localEnd.ToUniversalTime());
    }

    [Fact]
    [Trait("ColorConverter", "RoundTrip")]
    public async Task Color_Should_RoundTripIncludingAlpha()
    {
        var color = Color.FromArgb(120, 10, 200, 30);

        var tag = new Tag { Name = "Colored tag", Color = color };
        var category = new Category { Name = "Colored category", Color = color };
        await Resolve<ITagRepository>().AddAndSaveAsync(tag, TestContext.Current.CancellationToken);
        await Resolve<ICategoryRepository>().AddAndSaveAsync(category, TestContext.Current.CancellationToken);

        var reloadedTag = await ReloadAsync<Tag>(tag.Id);
        var reloadedCategory = await ReloadAsync<Category>(category.Id);

        reloadedTag.Color.ToArgb().Should().Be(color.ToArgb());
        reloadedCategory.Color.ToArgb().Should().Be(color.ToArgb());
    }

    [Fact]
    [Trait("ScheduledCategoryColor", "RoundTrip")]
    public async Task ScheduledCategoryColor_Should_RoundTripIncludingAlpha()
    {
        // ScheduledCategory uses its own inline ARGB converter, not the shared ColorConverter.
        var color = Color.FromArgb(64, 5, 6, 7);
        var date = new DateOnly(2026, 6, 1);

        var snapshot = new ScheduleSnapshot
        {
            UserId = CurrentUser.UserId,
            Date = date,
            ScheduledCategories =
            {
                new ScheduledCategory { UserId = CurrentUser.UserId, Date = date, Name = "Colored", Color = color }
            }
        };
        Db.Add(snapshot);
        await Db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var scheduledCategoryId = snapshot.ScheduledCategories.Single().Id;

        var reloaded = await ReloadAsync<ScheduledCategory>(scheduledCategoryId);

        reloaded.Color.ToArgb().Should().Be(color.ToArgb());
    }

    private async Task<TEntity> ReloadAsync<TEntity>(Guid id) where TEntity : class, IDbEntity<Guid>
    {
        // Drop tracked instances so the read materializes from the database and re-runs the converters.
        Db.ChangeTracker.Clear();
        return await Db.Set<TEntity>().FirstAsync(x => x.Id == id, TestContext.Current.CancellationToken);
    }
}
