namespace MongoDbPoC.Data;

public interface IEntityOptions
{
    string Collection { get; init; }
    Dictionary<string, string> Indexes { get; init; }
}
