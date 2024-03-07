using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<List<Review>> GetAllMyReviews(string userId);
    Task<List<Review>> GetProductReviews(string productId);
    Task DeleteUsersReview(string userId, string reviewId);
    Task<Review> GetReviewByIdWithUser(string reviewId);
    Task<List<Review>> GetAllReviewsWithUser(string? query = null);
}