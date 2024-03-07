using MongoDB.Driver;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public async Task DeleteAllCategories()
    {
        await Task.Run(async () =>
        {
            var filter = Builders<Category>.Filter.Empty;
            await DB.Collection<Category>().DeleteManyAsync(filter);
        });
    }

    public async Task DeleteFilterSectionAsync(string categoryId, string filterId)
    {
        var category = await DB.Find<Category>().OneAsync(categoryId);
        var filterData = category.FilterData.FirstOrDefault(f => f.ID == filterId);

        if (filterData != null)
        {
            category.FilterData.Remove(filterData);
            await category.SaveAsync();
        }
    }
}