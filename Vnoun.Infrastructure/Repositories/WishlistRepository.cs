using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
{
    public async Task DeleteAllByProductId(string productId, string userId)
    {
        var filter = Builders<Wishlist>.Filter.Eq(x => x.ProductId, productId);
        filter &= Builders<Wishlist>.Filter.Eq(x => x.UserId, userId);
        await DB.Collection<Wishlist>().DeleteManyAsync(filter);
    }

    public async Task DeleteAllItemsForUser(string userId)
    {
        var filter = Builders<Wishlist>.Filter.Eq(x => x.UserId, userId);
        var items = await DB.Collection<Wishlist>().Find(filter).ToListAsync();

        foreach (var item in items)
        {
            var product = await DB.Find<Product>().OneAsync(item.ProductId);
            product.wishes--;

            await product.SaveAsync();
            await item.DeleteAsync();
        }
    }

    public async Task DeleteWishListsByIdsForUser(string userId, List<string> ids)
    {
        foreach (var id in ids)
        {
            await DeleteAllByProductId(id, userId);
        }
    }

    public Task<List<Wishlist>> GetWishListsForUser(string userId, string? query = null)
    {
        if (query != null)
        {
            IMongoQueryable<Wishlist> queryable = DB.Collection<Wishlist>().AsQueryable();

            var features = new APIFeatures<Wishlist>(queryable, query);
            return features.Filter()
                           .Sort()
                           .Paginate()
                           .ToListAsync();
        }

        var filter = Builders<Wishlist>.Filter.Eq(x => x.UserId, userId);
        return DB.Collection<Wishlist>().Find(filter).ToListAsync();
    }
}