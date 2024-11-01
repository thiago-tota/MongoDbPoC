using System.Linq.Expressions;
using MongoDbPoC.Data.Entities;

namespace MongoDbPoC.Data.Repository;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAllQueryable();
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate, int limit, int skip, CancellationToken cancellationToken);
    Task<IEnumerable<TResult>> SearchAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector, int limit, int skip, CancellationToken cancellationToken);
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<IEnumerable<Guid>> CreateAsync(List<T> entities, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
