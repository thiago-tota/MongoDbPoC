namespace MongoDbPoC.Data;

public record UserEntityOptions : IEntityOptions
{
    public string Collection { get; init; } = "Users";
    public Dictionary<string, string> Indexes { get; init; } = new Dictionary<string, string>
{
    { "Name", "NameAscIndex" },
    { "UserType", "UserTypeAscIndex" },
    { "Email", "EmailAscIndex" }
};
}
