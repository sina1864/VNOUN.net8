using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<IEnumerable<Post>> GetPinnedAsync();
    Task<List<Post>> GetByPublisherAsync(string publisherId);
    Task<List<Post>> GetAllPostsWithPublisher(string queryString);
    Task<Post> GetPostWithPublisher(string postId);
}