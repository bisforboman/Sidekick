using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sidekick.Common;

public static class SerializationHelper
{
    private static readonly JsonSerializerOptions defaultJsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static T? FromJsonTo<T>(this string stream, JsonSerializerOptions? jsonSerializerOptions = null) => 
        JsonSerializer.Deserialize<T>(stream, jsonSerializerOptions ?? defaultJsonSerializerOptions);

    public static async Task<T?> FromJsonToAsync<T>(this Stream stream, JsonSerializerOptions? jsonSerializerOptions = null) => 
        await JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions ?? defaultJsonSerializerOptions);

    public static string ToJson<T>(this T obj, JsonSerializerOptions? jsonSerializerOptions = null) => 
        JsonSerializer.Serialize(obj, jsonSerializerOptions ?? defaultJsonSerializerOptions);

    public static async Task ToJsonAsync<T>(this Stream utf8Json, T obj) => 
        await JsonSerializer.SerializeAsync(utf8Json, obj);
}
