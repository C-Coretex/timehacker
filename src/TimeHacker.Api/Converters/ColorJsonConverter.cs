using System.Drawing;

namespace TimeHacker.Api.Converters;

/// <summary>
/// Serializes <see cref="Color"/> as its ARGB int32 — the same representation used for DB storage
/// (see Infrastructure ColorConverter). System.Drawing.Color has no writable properties, so STJ's
/// default object serialization cannot round-trip it; this converter makes it symmetric and exact.
/// </summary>
internal sealed class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Color.FromArgb(reader.GetInt32());

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value.ToArgb());
}
