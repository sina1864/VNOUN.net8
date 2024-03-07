using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System.Collections;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories.Base;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    public async Task<List<T>> GetAllAsync(string? queryParam = null)
    {
        if (queryParam != null)
        {
            IMongoQueryable<T> query = DB.Collection<T>().AsQueryable();

            var features = new APIFeatures<T>(query, queryParam);
            var items = await features.Filter()
                                .Sort()
                                .Paginate()
                                .ToListAsync();

            if (typeof(T) == typeof(Product))
            {
                foreach (var item in items)
                {
                    if (item is Product product)
                    {
                        var reviews = await DB.Find<Review>()
                            .Match(r => r.ProductId == product.ID)
                            .ExecuteAsync();

                        product.RatingsAverage = reviews.Count > 0 ? (double)reviews.Average(r => r.Rating) : 4.5;
                        product.RatingsQuantity = reviews.Count;
                    }
                }
            }

            return items;
        }

        return await DB.Find<T>().ExecuteAsync();
    }

    public async Task<List<T>> GetAllWithQueryParamAsync(int limit, int skip)
    {
        return await DB.Find<T>()
            .Limit(limit)
            .Skip(skip)
            .ExecuteAsync();
    }

    public async Task<T?> FindById(string id)
    {
        var result = await DB.Find<T>().OneAsync(id);

        if (result is Product product)
        {
            var reviews = await DB.Find<Review>()
                .Match(r => r.ProductId == product.ID)
                .ExecuteAsync();

            product.RatingsAverage = reviews.Count > 0 ? (double)reviews.Average(r => r.Rating) : 4.5;
            product.RatingsQuantity = reviews.Count;
        }

        return result;
    }

    public async Task CreateAsync(T entity)
    {
        await DB.InsertAsync(entity);
    }

    public async Task<T?> UpdateOneAsync(string id, T entity)
    {
        var existingEntity = await DB.Find<T>().OneAsync(id);

        foreach (var prop in typeof(T).GetProperties())
        {
            var newValue = prop.GetValue(entity);
            var existingValue = prop.GetValue(existingEntity);

            if (newValue == null || (newValue is IList newList && newList.Count == 0)) continue;

            if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                prop.SetValue(existingEntity, newValue);
            else if
                (!newValue.Equals(existingValue)) prop.SetValue(existingEntity, newValue);
        }

        await existingEntity.SaveAsync();

        return existingEntity;
    }

    public async Task DeleteOneAsync(string id)
    {
        await DB.DeleteAsync<T>(id);
    }

    public async Task DeleteAllAsync()
    {
        var filter = Builders<T>.Filter.Empty;
        await DB.Collection<T>().DeleteManyAsync(filter);
    }
}