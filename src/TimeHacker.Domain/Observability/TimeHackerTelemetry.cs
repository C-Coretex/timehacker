using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TimeHacker.Domain.Observability;

/// <summary>
/// Shared telemetry primitives for TimeHacker's application-level instrumentation. Lives in the Domain
/// layer so both the Application and Domain.Services layers can emit against a single <see cref="ActivitySource"/>
/// and <see cref="Meter"/>. The names below are registered with the OpenTelemetry SDK in the API's
/// <c>AddOpenTelemetry</c> wiring (<c>AddSource</c> / <c>AddMeter</c>).
/// </summary>
public static class TimeHackerTelemetry
{
    public const string ActivitySourceName = "TimeHacker";
    public const string MeterName = "TimeHacker";

    /// <summary>Business spans (e.g. timeline generation) nested inside the ambient request trace.</summary>
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    private static readonly Meter Meter = new(MeterName);

    /// <summary>
    /// Count of day-timeline requests, tagged <c>outcome = snapshot_hit | generated</c> — the snapshot
    /// snapshot-hit ratio is derived from this.
    /// </summary>
    public static readonly Counter<long> SnapshotRequests =
        Meter.CreateCounter<long>("timehacker.snapshots.requested", unit: "{request}",
            description: "Day-timeline requests, split by whether an existing snapshot was reused or a new one generated.");

    /// <summary>Wall-clock duration of the timeline-generation algorithm for a single day, in milliseconds.</summary>
    public static readonly Histogram<double> TimelineGenerationDuration =
        Meter.CreateHistogram<double>("timehacker.timeline.generation.duration", unit: "ms",
            description: "Duration of generating a single day's timeline (snapshot miss path).");

    /// <summary>Number of scheduled task instances produced by timeline generation.</summary>
    public static readonly Counter<long> ScheduledTasksGenerated =
        Meter.CreateCounter<long>("timehacker.scheduled_tasks.generated", unit: "{task}",
            description: "Scheduled task instances produced while generating timelines.");

    public const string OutcomeSnapshotHit = "snapshot_hit";
    public const string OutcomeGenerated = "generated";
}
