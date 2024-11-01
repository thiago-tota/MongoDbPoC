using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbPoC.Data.Entities;

namespace MongoDbPoC.Data.Repository;

public class MongoRepository2<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<TEntity> _collection;
    private readonly Dictionary<string, string> _indexes;

    public MongoRepository2(IOptions<MongoOptions> mongoDbOptions, IOptions<IEntityOptions> entityOptions)
    {
        _client = new MongoClient(mongoDbOptions.Value.Host);
        _database = _client.GetDatabase(mongoDbOptions.Value.Database);
        _collection = _database.GetCollection<TEntity>(entityOptions.Value.Collection);
        _indexes = entityOptions.Value.Indexes;
        CheckIndexes(new CancellationToken());
    }

    private void CheckIndexes(CancellationToken cancellationToken)
    {
        var existingIndexes = (_collection.Indexes.List(cancellationToken)).ToList(cancellationToken);

        foreach (var item in _indexes)
        {
            if (!existingIndexes.Any(f => f["key"].AsBsonDocument.Contains(item.Key)))
            {
                var indexKey = Builders<TEntity>.IndexKeys.Ascending(item.Key);
                var indexModel = new CreateIndexModel<TEntity>(indexKey, new CreateIndexOptions { Name = item.Value });
                _collection.Indexes.CreateOne(indexModel, cancellationToken: cancellationToken);
            }
        }
    }

    public IQueryable<TEntity> GetAllQueryable()
    {
        throw new NotImplementedException();
        //return await _collection.Find(_ => true).ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const int limit = 1;
        const int skip = 0;
        return (await SearchAsync(f => f.Id == id, limit, skip, cancellationToken)).FirstOrDefault()!;
    }

    public async Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, int limit, int skip, CancellationToken cancellationToken)
    {
        return await _collection.Find(predicate).ToListAsync(cancellationToken);
    }

    public Task<IEnumerable<TResult>> SearchAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, int limit, int skip, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(entity, null, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Guid>> CreateAsync(List<TEntity> entities, CancellationToken cancellationToken)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        return entities.Select(f => f.Id).ToList();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var result = await _collection.ReplaceOneAsync(f => f.Id == entity.Id, entity, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0 ? entity : default!;
    }

    public async Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _collection.DeleteOneAsync(f => f.Id == id, cancellationToken);
        return result.DeletedCount > 0 ? id : Guid.Empty;
    }
}
