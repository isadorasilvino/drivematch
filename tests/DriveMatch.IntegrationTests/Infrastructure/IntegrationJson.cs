using System.Text.Json;
using System.Text.Json.Serialization;

namespace DriveMatch.IntegrationTests.Infrastructure;

public static class IntegrationJson
{
    public static JsonSerializerOptions Options { get; } =
        CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web);

        options.Converters.Add(
            new JsonStringEnumConverter());

        return options;
    }
}