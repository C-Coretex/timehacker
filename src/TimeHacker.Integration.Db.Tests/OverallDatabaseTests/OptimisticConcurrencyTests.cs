using Timehacker.Integration.Db.Tests;
using TimeHacker.Integration.Db.Tests.Fixtures;

namespace TimeHacker.Integration.Db.Tests.OverallDatabaseTests;

public class OptimisticConcurrencyTests(DbContainerFixture fixture) : DbIntegrationTestBase(fixture)
{
}
