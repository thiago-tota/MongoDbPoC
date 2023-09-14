using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDbPoC
{
    internal abstract class MongoDbRepository<TDto>
    {
        private readonly string _host;
        private readonly string _databaseName;
        private readonly string _collectionName;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _collection;

        public abstract Dictionary<string, string> Indexes { get; }

        public MongoDbRepository(string host, string database, string collection)
        {
            _host = host;
            _databaseName = database;
            _collectionName = collection;

            _client = new MongoClient(_host);
            _database = _client.GetDatabase(_databaseName);
            _collection = _database.GetCollection<BsonDocument>(_collectionName);
        }

        public virtual async Task CheckIndexes()
        {
            var existingIndexes = (await _collection.Indexes.ListAsync()).ToList();

            foreach (var item in Indexes)
            {
                if (!existingIndexes.Any(f => f["key"].AsBsonDocument.Contains(item.Key)))
                {
                    var indexKey = Builders<BsonDocument>.IndexKeys.Ascending(item.Key);
                    var indexModel = new CreateIndexModel<BsonDocument>(indexKey, new CreateIndexOptions { Name = item.Value });
                    await _collection.Indexes.CreateOneAsync(indexModel);
                }
            }
        }

        public virtual async Task CreateAsync(TDto document)
        {
            await _collection.InsertOneAsync(document.ToBsonDocument());
        }

        public virtual void Save(List<TDto> records)
        {
            var bsonDocuments = records.Select(r => r.ToBsonDocument());
            _collection.InsertMany(bsonDocuments);
        }

        public virtual List<TDto>? SearchByField(string field, int value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(field, value);
            var result = _collection.Find(filter).ToList();
            return result.Select(r => BsonSerializer.Deserialize<TDto>(r)).ToList();
        }

        public virtual async Task<List<TDto>> ReadAllAsync()
        {
            var result = await _collection.Find(_ => true).ToListAsync();
            return result.Select(r => BsonSerializer.Deserialize<TDto>(r)).ToList();
        }

        public virtual async Task UpdateAsync(string id, TDto document)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, document.ToBsonDocument());
        }

        public virtual async Task DeleteAsync(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
