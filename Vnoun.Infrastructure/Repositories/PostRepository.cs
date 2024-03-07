using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class PostRepository : Repository<Post>, IPostRepository
{
    public Task<IEnumerable<Post>> GetPinnedAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<Post>> GetByPublisherAsync(string publisherId)
    {
        var posts = await DB.Find<Post>()
            .Match(p => p.PublisherId == ObjectId.Parse(publisherId))
            .ExecuteAsync();

        foreach (var post in posts)
        {
            var publisher = await DB.Find<User>().OneAsync(post.PublisherId.ToString());
            post.Publisher = publisher;
        }

        return posts;
    }

    public async Task<List<Post>> GetAllPostsWithPublisher(string? queryString)
    {
        _ = new List<Post>();
        List<Post> posts;

        if (queryString == null)
        {
            posts = await DB.Find<Post>().ExecuteAsync();
        }
        else
        {
            IMongoQueryable<Post> query = DB.Collection<Post>().AsQueryable();
            var features = new APIFeatures<Post>(query, queryString);
            posts = await features.Filter().Sort().Paginate().ToListAsync();
        }

        foreach (var post in posts)
        {
            var publisher = await DB.Find<User>().OneAsync(post.PublisherId.ToString());
            post.Publisher = publisher;
        }

        return posts;
    }

    public async Task<Post> GetPostWithPublisher(string postId)
    {
        var post = await DB.Find<Post>().OneAsync(postId);

        var publisher = DB.Find<User>().OneAsync(post.PublisherId.ToString());
        post.Publisher = publisher.Result;

        return post;
    }
}