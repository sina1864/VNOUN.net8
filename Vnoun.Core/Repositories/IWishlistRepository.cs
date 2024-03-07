using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface IWishlistRepository : IRepository<Wishlist>
{
    Task DeleteAllItemsForUser(string userId);
    Task<List<Wishlist>> GetWishListsForUser(string userId, string? query = null);
    Task DeleteAllByProductId(string productId, string userId);
    Task DeleteWishListsByIdsForUser(string userId, List<string> ids);
}