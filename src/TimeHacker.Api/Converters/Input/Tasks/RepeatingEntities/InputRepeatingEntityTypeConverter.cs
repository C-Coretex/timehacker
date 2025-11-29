using TimeHacker.Api.Models.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities
{
    public class InputRepeatingEntityTypeConverter : JsonConverter<IInputRepeatingEntityType>
    {
        public override IInputRepeatingEntityType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

        if (!doc.RootElement.TryGetProperty(nameof(IInputRepeatingEntityType.EntityType), out var typeProp))
            throw new JsonException("Missing 'type' discriminator");

        var typeString = typeProp.GetString()!;
        var typeEnum = Enum.Parse<RepeatingEntityTypeEnum>(typeString);

        var json = doc.RootElement.GetRawText();

        return typeEnum switch
        {
            RepeatingEntityTypeEnum.DayRepeatingEntity => JsonSerializer.Deserialize<InputDayRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.WeekRepeatingEntity => JsonSerializer.Deserialize<InputWeekRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.MonthRepeatingEntity => JsonSerializer.Deserialize<InputMonthRepeatingEntityModel>(json, options),
            RepeatingEntityTypeEnum.YearRepeatingEntity => JsonSerializer.Deserialize<InputYearRepeatingEntityModel>(json, options),
            _ => throw new JsonException($"Unknown type: {typeString}")
        };
        }

        public override void Write(Utf8JsonWriter writer, IInputRepeatingEntityType value, JsonSerializerOptions options)
        {
            //will work, because it will serialize the concrete type, not the interface
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
