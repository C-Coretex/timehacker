using System.Text.Json.Nodes;
using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;

public class InputRepeatingEntityTypeConverter : JsonConverter<InputRepeatingEntityModelBase>
{
    public override InputRepeatingEntityModelBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);

        if (!doc.RootElement.TryGetProperty(nameof(InputRepeatingEntityModelBase.EntityType), out var typeProp))
            throw new JsonException("Missing 'EntityType' discriminator");

        var typeString = typeProp.GetRawText();
        var typeEnum = Enum.Parse<RepeatingEntityTypeEnum>(typeString);

        // Parse as JsonNode and remove the EntityType property to avoid deserializing it
        var jsonNode = JsonNode.Parse(doc.RootElement.GetRawText())!.AsObject();
        jsonNode.Remove(nameof(InputRepeatingEntityModelBase.EntityType));
        var json = jsonNode.ToJsonString();

        return typeEnum switch
        {
            RepeatingEntityTypeEnum.DayRepeatingEntity => JsonSerializer.Deserialize<InputDayRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.WeekRepeatingEntity => JsonSerializer.Deserialize<InputWeekRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.MonthRepeatingEntity => JsonSerializer.Deserialize<InputMonthRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.YearRepeatingEntity => JsonSerializer.Deserialize<InputYearRepeatingEntityModel>(json, options),
            _ => throw new JsonException($"Unknown type: {typeEnum}")
        };
    }

    public override void Write(Utf8JsonWriter writer, InputRepeatingEntityModelBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
