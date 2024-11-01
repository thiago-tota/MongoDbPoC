using System.Text.Json.Serialization;

namespace MongoDbPoC.Data.Entities;

public abstract record BaseEntity
{
    [JsonPropertyName("_id")]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
