using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace MongoDbPoC.Tests
{
    public class MongoRepositoryFixture
    {
        private IConfigurationRoot? ConfigurationRoot { get; set; }
        private bool IsMongoReset { get; set; }

        internal IConfigurationRoot GetConfiguration()
        {
            if (ConfigurationRoot == null)
            {
                ConfigurationRoot = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            }

            return ConfigurationRoot;
        }

        internal void ResetDb(string host, string dbName, string collectionName)
        {
            if (IsMongoReset)
                return;

            var client = new MongoClient(host);
            var database = client.GetDatabase(dbName);
            database.DropCollection(collectionName);
            IsMongoReset = true;
        }
    }
}
