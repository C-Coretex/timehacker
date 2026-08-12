using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace TimeHacker.Domain.Observability;

/// <summary>
/// Counts <b>distinct</b> users active over a few rolling windows and publishes them as a single
/// observable gauge (<c>timehacker.users.active</c>, tagged <c>window</c>).
/// <para>
/// The point of this type is cardinality: tagging a metric with a user id would create one series per user
/// and is never acceptable. Keeping the set of ids in memory and exporting only its size gives the
/// "how many people use this" number at a fixed cost of <see cref="Windows"/>.Length series.
/// </para>
/// <para>
/// Caveat: the set is <b>in-process</b>. It is empty after a restart and would double-count a user active on
/// two instances if the API is ever scaled out. For exact counts over an arbitrary range, aggregate the
/// <c>enduser_id</c> field on log records in Loki instead — that path is authoritative, this one is live.
/// </para>
/// <para>
/// Static (rather than DI-registered) to match <see cref="TimeHackerTelemetry"/>, and because registering the
/// gauge per instance would duplicate the instrument in hosts that build more than one service provider.
/// </para>
/// </summary>
public static class ActiveUserTracker
{
    /// <summary>Rolling windows reported by the gauge. Each one costs exactly one series.</summary>
    private static readonly (string Label, long Milliseconds)[] Windows =
    [
        ("5m", (long)TimeSpan.FromMinutes(5).TotalMilliseconds),
        ("1h", (long)TimeSpan.FromHours(1).TotalMilliseconds),
        ("24h", (long)TimeSpan.FromHours(24).TotalMilliseconds)
    ];

    /// <summary>
    /// Hard ceiling on tracked ids so a long-running process (or a flood of distinct logins) can't grow the
    /// map without bound. Beyond it, only already-known users keep being refreshed.
    /// </summary>
    private const int MaxTrackedUsers = 50_000;

    private const string WindowTagName = "window";

    // userId -> Environment.TickCount64 at last request. TickCount64 is monotonic, so it is immune to
    // wall-clock adjustments that could otherwise make an entry look arbitrarily old or new.
    private static readonly ConcurrentDictionary<Guid, long> LastSeen = new();

    private static readonly ObservableGauge<int> ActiveUsers =
        TimeHackerTelemetry.Meter.CreateObservableGauge("timehacker.users.active", ObserveActiveUsers,
            unit: "{user}",
            description: "Distinct users seen in the last 5 minutes / hour / day (in-process, resets on restart).");

    /// <summary>
    /// Registers the gauge at startup rather than on the first login, so the series reads a real zero from
    /// the moment the app starts instead of being absent until someone signs in.
    /// </summary>
    public static void EnsureInitialized() => _ = ActiveUsers;

    /// <summary>Records that <paramref name="userId"/> just made a request. Called once per authenticated request.</summary>
    public static void Touch(Guid userId)
    {
        // Only new keys are rejected at the ceiling — existing users must keep their timestamp fresh,
        // otherwise a full map would age every user out and report zero.
        if (LastSeen.Count >= MaxTrackedUsers && !LastSeen.ContainsKey(userId))
            return;

        LastSeen[userId] = Environment.TickCount64;
    }

    /// <summary>
    /// Invoked by the metrics SDK on each collection: drops ids older than the widest window (this is the
    /// only place the map shrinks) and reports the surviving count per window.
    /// </summary>
    private static IEnumerable<Measurement<int>> ObserveActiveUsers()
    {
        var now = Environment.TickCount64;
        var widestWindow = Windows.Max(w => w.Milliseconds);

        var ages = new List<long>(LastSeen.Count);
        foreach (var (userId, lastSeen) in LastSeen)
        {
            var age = now - lastSeen;
            if (age > widestWindow)
            {
                // Remove only if nothing refreshed it in the meantime, so a concurrent Touch isn't lost.
                LastSeen.TryRemove(new KeyValuePair<Guid, long>(userId, lastSeen));
                continue;
            }

            ages.Add(age);
        }

        return Windows.Select(window => new Measurement<int>(
            ages.Count(age => age <= window.Milliseconds),
            new KeyValuePair<string, object?>(WindowTagName, window.Label)));
    }
}
