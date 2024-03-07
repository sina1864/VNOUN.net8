using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> GetProductsByFilter(Func<Find<Product, Product>, Find<Product, Product>>? sortQuery);
    Task DeleteColorAsync(string id, string colorName);
    Task<List<Product>> GetMostWishListedProductsService(int page, int pageSize);
    Task<List<Product>> GetMostAddedToCardService(int page, int pageSize);
    Task LowerWishedCount(string productId);
    Task IncreaseWishedCount(string productId);
    Task LowerAddedToCardCount(string productId);
    Task IncreaseAddedToCardCount(string productId);
}