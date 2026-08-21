namespace TimeHacker.Api.Converters;

/// <summary>
/// Normalises every <see cref="DateTime"/> crossing the API boundary to <see cref="DateTimeKind.Utc"/>.
/// STJ's default binding is Kind-sensitive — a naive timestamp yields <c>Unspecified</c>, a trailing 'Z'
/// yields <c>Utc</c>, and an explicit offset is converted to <c>Local</c> — so the same instant would be
/// stored differently depending on the payload's format and the server's timezone.
/// </summary>
internal sealed class DateTimeUtcJsonConverter : JsonConverter<DateTime>
{
    // Honour an explicit offset/'Z', and treat a naive timestamp as UTC (not server-local) so storage is
    // deterministic regardless of where the app runs.
    private const DateTimeStyles UtcStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, UtcStyles, out var parsed))
            throw new JsonException($"'{value}' is not a valid date-time.");

        return parsed;
    }

    // Round-trip format 'O' on a Utc value emits the trailing 'Z', so Read above returns the same instant.
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
}
