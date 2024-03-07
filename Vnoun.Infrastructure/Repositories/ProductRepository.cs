using MongoDB.Driver;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public async Task<List<Product>> GetProductsByFilter(Func<Find<Product, Product>, Find<Product, Product>>? sortQuery)
    {
        var query = DB.Find<Product, Product>();

        if (sortQuery != null) query = sortQuery(query);

        var products = await query
            .Limit(7)
            .Project(p => new Product
            {
                ID = p.ID,
                RatingsAverage = p.RatingsAverage,
                Description = p.Description,
                RatingsQuantity = p.RatingsQuantity,
                CreatedAt = p.CreatedAt,
                Name = p.Name,
                Colors = p.Colors,
                Category = p.Category,
                GlobalDiscount = p.GlobalDiscount
            }).ExecuteAsync();

        foreach (var product in products)
        {
            var reviews = await DB.Find<Review>()
                .Match(r => r.ProductId == product.ID)
                .ExecuteAsync();

            product.RatingsAverage = (reviews.Count > 0 ? (double)reviews.Average(r => r.Rating) : 4.5);
            product.RatingsQuantity = reviews.Count;
        }

        return products.ToList();
    }

    public async Task DeleteColorAsync(string id, string colorName)
    {
        var product = await DB.Find<Product>().OneAsync(id);
        var color = product.Colors.FirstOrDefault(f => f.ColorName == colorName);

        if (color != null)
        {
            product.Colors.Remove(color);
            await product.SaveAsync();
        }
    }

    public async Task<List<Product>> GetMostWishListedProductsService(int page, int pageSize)
    {
        var products = await DB.Find<Product>()
            .Sort(p => p.wishes, MongoDB.Entities.Order.Descending)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .Project(p => new Product
            {
                ID = p.ID,
                RatingsAverage = p.RatingsAverage,
                Description = p.Description,
                RatingsQuantity = p.RatingsQuantity,
                CreatedAt = p.CreatedAt,
                Name = p.Name,
                Colors = p.Colors,
                Category = p.Category,
                GlobalDiscount = p.GlobalDiscount
            }).ExecuteAsync();

        foreach (var product in products)
        {
            var reviews = await DB.Find<Review>()
                .Match(r => r.ProductId == product.ID)
                .ExecuteAsync();

            product.RatingsAverage = (reviews.Count > 0 ? (double)reviews.Average(r => r.Rating) : 4.5);
            product.RatingsQuantity = reviews.Count;
        }

        return products.ToList();
    }

    public async Task<List<Product>> GetMostAddedToCardService(int page, int pageSize)
    {
        var prods = await DB.Find<Product>()
            .Sort(p => p.card_adds, MongoDB.Entities.Order.Descending)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .Project(p => new Product
            {
                ID = p.ID,
                RatingsAverage = p.RatingsAverage,
                Description = p.Description,
                RatingsQuantity = p.RatingsQuantity,
                CreatedAt = p.CreatedAt,
                Name = p.Name,
                Colors = p.Colors,
                Category = p.Category,
                GlobalDiscount = p.GlobalDiscount
            }).ExecuteAsync();

        foreach (var product in prods)
        {
            var reviews = await DB.Find<Review>()
                .Match(r => r.ProductId == product.ID)
                .ExecuteAsync();

            product.RatingsAverage = (reviews.Count > 0 ? (double)reviews.Average(r => r.Rating) : 4.5);
            product.RatingsQuantity = reviews.Count;
        }

        return prods.ToList();
    }

    public async Task LowerWishedCount(string productId)
    {
        var product = await DB.Find<Product>().OneAsync(productId);

        if (product.wishes > 0)
        {
            product.wishes--;
            await product.SaveAsync();
        }
    }

    public async Task IncreaseWishedCount(string productId)
    {
        var product = await DB.Find<Product>().OneAsync(productId);
        product.wishes++;
        await product.SaveAsync();
    }

    public async Task LowerAddedToCardCount(string productId)
    {
        var product = await DB.Find<Product>().OneAsync(productId);

        if (product.card_adds > 0)
        {
            product.card_adds--;
            await product.SaveAsync();
        }
    }

    public async Task IncreaseAddedToCardCount(string productId)
    {
        var product = await DB.Find<Product>().OneAsync(productId);
        product.card_adds++;
        await product.SaveAsync();
    }
}