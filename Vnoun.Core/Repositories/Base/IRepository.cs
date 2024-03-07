namespace Vnoun.Core.Repositories.Base;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(string queryParam = null);
    Task<List<T>> GetAllWithQueryParamAsync(int limit, int skip);
    Task<T?> FindById(string id);
    Task CreateAsync(T entity);
    Task<T?> UpdateOneAsync(string id, T entity);
    Task DeleteOneAsync(string id);
    Task DeleteAllAsync();
}