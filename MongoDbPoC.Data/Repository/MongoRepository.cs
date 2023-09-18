using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDbPoC.Data.Entities;

namespace MongoDbPoC.Data.Repository
{
    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity
    {
        private readonly Dictionary<string, string> _indexes;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(string host, string database, string collection, Dictionary<string, string> indexes)
        {
            _indexes = indexes;
            _client = new MongoClient(host);
            _database = _client.GetDatabase(database);
            _collection = _database.GetCollection<T>(collection);
        }

        public async Task CheckIndexes()
        {
            var existingIndexes = (await _collection.Indexes.ListAsync()).ToList();

            foreach (var item in _indexes)
            {
                if (!existingIndexes.Any(f => f["key"].AsBsonDocument.Contains(item.Key)))
                {
                    var indexKey = Builders<T>.IndexKeys.Ascending(item.Key);
                    var indexModel = new CreateIndexModel<T>(indexKey, new CreateIndexOptions { Name = item.Value });
                    await _collection.Indexes.CreateOneAsync(indexModel);
                }
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return (await GetByFieldAsync(f => f.Id == id)).FirstOrDefault()!;
        }

        public async Task<IEnumerable<T>> GetByFieldAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).ToListAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task CreateAsync(IEnumerable<T> entities)
        {
            var documents = entities.Select(r => r);
            await _collection.InsertManyAsync(documents);
        }

        public async Task<long> UpdateAsync(Guid id, T entity)
        {
            var result = await _collection.ReplaceOneAsync(f => f.Id == id, entity);
            return result.ModifiedCount;
        }

        public async Task<long> DeleteAsync(Guid id)
        {
            var result = await _collection.DeleteOneAsync(f => f.Id == id);
            return result.DeletedCount;
        }
    }
}
