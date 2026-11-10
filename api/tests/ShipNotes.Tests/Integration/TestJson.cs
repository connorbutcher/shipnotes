using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShipNotes.Tests.Integration;

/// <summary>JSON options matching the API: web defaults with enums as camelCase names.</summary>
internal static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) },
    };
}
