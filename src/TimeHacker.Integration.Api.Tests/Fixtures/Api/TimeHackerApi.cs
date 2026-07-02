using Refit;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

/// <summary>
/// Composite typed client over the whole API: one Refit sub-client per controller, all sharing the same
/// (cookie-carrying) HttpClient. Grouping the endpoints by controller keeps each surface small and lets tests
/// read as api.Categories.CreateCategory(...), api.Tasks.CreateSchedule(...), etc.
/// </summary>
public sealed class TimeHackerApi
{
    public TimeHackerApi(HttpClient httpClient)
    {
        Auth = RestService.For<IAuthApi>(httpClient, RefitConfig.Settings);
        Health = RestService.For<IHealthApi>(httpClient, RefitConfig.Settings);
        Categories = RestService.For<ICategoriesApi>(httpClient, RefitConfig.Settings);
        FixedTasks = RestService.For<IFixedTasksApi>(httpClient, RefitConfig.Settings);
        DynamicTasks = RestService.For<IDynamicTasksApi>(httpClient, RefitConfig.Settings);
        Users = RestService.For<IUsersApi>(httpClient, RefitConfig.Settings);
        Tasks = RestService.For<ITasksApi>(httpClient, RefitConfig.Settings);
    }

    public IAuthApi Auth { get; }
    public IHealthApi Health { get; }
    public ICategoriesApi Categories { get; }
    public IFixedTasksApi FixedTasks { get; }
    public IDynamicTasksApi DynamicTasks { get; }
    public IUsersApi Users { get; }
    public ITasksApi Tasks { get; }
}
