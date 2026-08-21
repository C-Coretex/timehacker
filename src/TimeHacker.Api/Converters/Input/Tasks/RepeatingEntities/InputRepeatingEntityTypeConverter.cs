using System.Text.Json.Nodes;
using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;

/// <summary>
/// Deserializes the incoming repeating-entity model by its "EntityType" discriminator into the matching
/// concrete Input*RepeatingEntityModel.
/// </summary>
internal sealed class InputRepeatingEntityTypeConverter : JsonConverter<InputRepeatingEntityModelBase>
{
    public override InputRepeatingEntityModelBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);

        // Honour the configured naming policy so the discriminator matches the rest of the payload's casing.
        var discriminatorName = options.PropertyNamingPolicy?.ConvertName(nameof(InputRepeatingEntityModelBase.EntityType))
                                ?? nameof(InputRepeatingEntityModelBase.EntityType);

        if (!doc.RootElement.TryGetProperty(discriminatorName, out var typeProp))
            throw new JsonException($"Missing '{discriminatorName}' discriminator");

        var typeString = typeProp.GetRawText();
        var typeEnum = Enum.Parse<RepeatingEntityType>(typeString);

        // Remove the discriminator before deserializing so it isn't bound onto the concrete subtype.
        var jsonNode = JsonNode.Parse(doc.RootElement.GetRawText())!.AsObject();
        jsonNode.Remove(discriminatorName);

        var json = jsonNode.ToJsonString();

        return typeEnum switch
        {
            RepeatingEntityType.DayRepeatingEntity => JsonSerializer.Deserialize<InputDayRepeatingEntityModel>(json, options),
            RepeatingEntityType.WeekRepeatingEntity => JsonSerializer.Deserialize<InputWeekRepeatingEntityModel>(json, options),
            RepeatingEntityType.MonthRepeatingEntity => JsonSerializer.Deserialize<InputMonthRepeatingEntityModel>(json, options),
            RepeatingEntityType.YearRepeatingEntity => JsonSerializer.Deserialize<InputYearRepeatingEntityModel>(json, options),
            RepeatingEntityType.OnceRepeatingEntity => JsonSerializer.Deserialize<InputOnceRepeatingEntityModel>(json, options),
            _ => throw new JsonException($"Unknown type: {typeEnum}")
        };
    }

    public override void Write(Utf8JsonWriter writer, InputRepeatingEntityModelBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
