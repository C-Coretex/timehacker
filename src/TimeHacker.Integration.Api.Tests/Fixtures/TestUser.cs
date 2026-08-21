using System.Text.Json;
using TimeHacker.Api.Converters;
using TimeHacker.Api.Converters.Input.Tasks.RepeatingEntities;

namespace TimeHacker.Integration.Api.Tests.Fixtures;

internal static class TestUser
{
    // Password satisfies the Identity policy (digit, upper, lower, len>=6).
    public static (string Email, string Password) New()
        => ($"user-{Guid.NewGuid():N}@test.local", "Passw0rd!");
}

internal static class RefitConfig
{
    // Mirror the API's own JSON setup so request bodies — the polymorphic RepeatingEntity models and
    // System.Drawing.Color — serialize exactly the way the controllers read them. Reads stay
    // case-insensitive (web defaults), so camelCase responses map back onto the typed models.
    private static readonly JsonSerializerOptions JsonOptions = BuildJsonOptions();

    public static readonly RefitSettings Settings = new()
    {
        ContentSerializer = new SystemTextJsonContentSerializer(JsonOptions)
    };

    private static JsonSerializerOptions BuildJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new InputRepeatingEntityTypeConverter());
        options.Converters.Add(new ColorJsonConverter());
        options.Converters.Add(new DateTimeUtcJsonConverter());
        return options;
    }
}
