using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoDbPoC.Data.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        private readonly Dictionary<string, string> _indexes;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoRepository(string host, string database, string collection, Dictionary<string, string> indexes)
        {
            _indexes = indexes;
            _client = new MongoClient(host);
            _database = _client.GetDatabase(database);
            _collection = _database.GetCollection<BsonDocument>(collection);
        }

        public async Task CheckIndexes()
        {
            var existingIndexes = (await _collection.Indexes.ListAsync()).ToList();

            foreach (var item in _indexes)
            {
                if (!existingIndexes.Any(f => f["key"].AsBsonDocument.Contains(item.Key)))
                {
                    var indexKey = Builders<BsonDocument>.IndexKeys.Ascending(item.Key);
                    var indexModel = new CreateIndexModel<BsonDocument>(indexKey, new CreateIndexOptions { Name = item.Value });
                    await _collection.Indexes.CreateOneAsync(indexModel);
                }
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await _collection.Find(_ => true).ToListAsync();
            return result.Select(f => BsonSerializer.Deserialize<T>(f));
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return (await GetByFieldAsync("_id", id)).FirstOrDefault()!;
        }

        public async Task<IEnumerable<T>> GetByFieldAsync(string field, object value)
        {
            var filter = Builders<BsonDocument>.Filter.Eq(field, value);
            var result = await _collection.Find(filter).ToListAsync();
            return result.Select(r => BsonSerializer.Deserialize<T>(r)).ToList();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity.ToBsonDocument());
        }

        public async Task CreateAsync(IEnumerable<T> entities)
        {
            var documents = entities.Select(r => r.ToBsonDocument());
            await _collection.InsertManyAsync(documents);
        }

        public async Task<long> UpdateAsync(Guid id, T entity)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await _collection.ReplaceOneAsync(filter, entity.ToBsonDocument());
            return result.ModifiedCount;
        }

        public async Task<long> DeleteAsync(Guid id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var result = await _collection.DeleteOneAsync(filter);
            return result.DeletedCount;
        }
    }
}
