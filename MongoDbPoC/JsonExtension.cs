using System.Text.Json;
using System.Text.Json.Serialization;

namespace MongoDbPoC
{
    internal static class JsonExtension
    {
        public static string ToJson<TValue>(this TValue value, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            return JsonSerializer.Serialize(value, jsonSerializerOptions);
        }

        public static JsonSerializerOptions GetApplicationDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}
