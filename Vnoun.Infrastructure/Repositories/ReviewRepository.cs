using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    IProductRepository _productRepository;
    public ReviewRepository(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task DeleteUsersReview(string userId, string reviewId)
    {
        var review = await DB.Find<Review>()
            .Match(r => r.ID == reviewId && r.UserId == userId)
            .ExecuteFirstAsync() ?? throw new Exception("Review not found");
        await review.DeleteAsync();
    }

    public async Task<List<Review>> GetAllMyReviews(string userId)
    {
        var reviews = await DB.Find<Review>()
            .Match(u => u.UserId == userId)
            .ExecuteAsync();

        foreach (var review in reviews)
        {
            review.User = await DB.Find<User>()
                .Match(u => u.ID == review.UserId)
                .ExecuteFirstAsync();
        }

        return reviews;
    }

    public async Task<List<Review>> GetAllReviewsWithUser(string? query = null)
    {
        if (query != null)
        {
            IMongoQueryable<Review> queryable = DB.Collection<Review>().AsQueryable();

            var features = new APIFeatures<Review>(queryable, query);
            return await features.Filter()
                                .Sort()
                                .Paginate()
                                .ToListAsync();
        }

        var reviews = await DB.Find<Review>()
            .ExecuteAsync();

        foreach (var review in reviews)
        {
            review.User = await DB.Find<User>()
                .Match(u => u.ID == review.UserId)
                .ExecuteFirstAsync();
        }

        return reviews;
    }

    public Task<List<Review>> GetProductReviews(string productId)
    {
        var reviews = DB.Find<Review>()
            .Match(r => r.ProductId == productId)
            .ExecuteAsync();

        foreach (var review in reviews.Result)
        {
            review.User = DB.Find<User>()
                .Match(u => u.ID == review.UserId)
                .ExecuteFirstAsync().Result;
        }

        return reviews;
    }

    public async Task<Review> GetReviewByIdWithUser(string reviewId)
    {
        var review = await DB.Find<Review>()
            .Match(r => r.ID == reviewId)
            .ExecuteFirstAsync();

        var user = DB.Find<User>()
            .Match(u => u.ID == review.UserId)
            .ExecuteFirstAsync();

        review.User = await user;

        return review;
    }
}