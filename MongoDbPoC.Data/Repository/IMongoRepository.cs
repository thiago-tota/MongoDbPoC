namespace MongoDbPoC.Data.Repository
{
    public interface IMongoRepository<T> where T : class
    {
        Task CheckIndexes();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetByFieldAsync(string field, object value);
        Task CreateAsync(T entity);
        Task CreateAsync(IEnumerable<T> entities);
        Task<long> UpdateAsync(Guid id, T entity);
        Task<long> DeleteAsync(Guid id);
    }
}
