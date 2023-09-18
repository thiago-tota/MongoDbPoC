using System.Text.Json.Serialization;

namespace MongoDbPoC.Data.Entities
{
    public interface IEntity
    {
        [JsonPropertyName("_id")]
        Guid Id { get; set; }
    }
}
