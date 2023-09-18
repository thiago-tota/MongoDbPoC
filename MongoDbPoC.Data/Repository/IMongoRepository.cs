using System.Linq.Expressions;
using MongoDbPoC.Data.Entities;

namespace MongoDbPoC.Data.Repository
{
    public interface IMongoRepository<T> where T : IEntity
    {
        Task CheckIndexes();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetByFieldAsync(Expression<Func<T, bool>> predicate);
        Task CreateAsync(T entity);
        Task CreateAsync(IEnumerable<T> entities);
        Task<long> UpdateAsync(Guid id, T entity);
        Task<long> DeleteAsync(Guid id);
    }
}
