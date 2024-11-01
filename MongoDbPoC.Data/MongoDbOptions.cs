namespace MongoDbPoC.Data;

public record MongoOptions
{
    public string Host { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
}
