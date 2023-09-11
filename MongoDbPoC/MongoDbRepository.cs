using System.Collections;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDbPoC
{
    internal class MongoDbRepository
    {
        const string _mondoDbUrl = "mongodb://localhost:3017";
        const string _mongoDatabaseName = "MongoDbLab";
        const string _mongoCollectionName = "MyDtoCollection";

        readonly MongoClient _mongoClient;
        readonly IMongoDatabase _mongoDatabase;
        readonly IMongoCollection<BsonDocument> _mongoCollection;

        public MongoDbRepository()
        {
            _mongoClient = new MongoClient(_mondoDbUrl);
            _mongoDatabase = _mongoClient.GetDatabase(_mongoDatabaseName);
            _mongoCollection = _mongoDatabase.GetCollection<BsonDocument>(_mongoCollectionName);
        }

        public async Task CheckIndexes()
        {
            var indexes = new Dictionary<string, string>
            {
                { "Locator", "LocatorAscIndex" },
                { "Name", "NameAscIndex" },
                { "DateOfBirth", "DateOfBirthAscIndex" },
                { "IsActive", "IsActiveAscIndex" }
            };

            var existingIndexes = (await _mongoCollection.Indexes.ListAsync()).ToList();

            foreach (var item in indexes)
            {
                if (!existingIndexes.Any(f => f["key"].AsBsonDocument.Contains(item.Key)))
                {
                    var indexKey = Builders<BsonDocument>.IndexKeys.Ascending(item.Key);
                    var indexModel = new CreateIndexModel<BsonDocument>(indexKey, new CreateIndexOptions { Name = item.Value });
                    await _mongoCollection.Indexes.CreateOneAsync(indexModel);
                }
            }
        }

        public void Save(List<MyDTO> records)
        {
            var bsonDocuments = records.Select(r => r.ToBsonDocument());
            _mongoCollection.InsertMany(bsonDocuments);
        }

        public List<MyDTO>? SearchByField(string field, int value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(field, value);
            var result = _mongoCollection.Find(filter).ToList();
            return result.Select(r => BsonSerializer.Deserialize<MyDTO>(r)).ToList();
        }
    }
}
