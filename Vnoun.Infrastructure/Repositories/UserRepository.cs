using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;
using Vnoun.Core.Repositories;
using Vnoun.Core.Repositories.Dtos.Requests.Auth;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        var user = await DB.Find<User>()
            .Match(p => p.Email == email)
            .ExecuteFirstAsync();
        return user;
    }

    public async Task<User?> UpdateUserService(string userId, UpdateUserServiceRequestDto requestDto)
    {
        try
        {
            var user = await DB.Find<User>().OneAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (requestDto.Email != null)
            {
                user.Email = requestDto.Email;
            }

            if (requestDto.Name != null)
            {
                user.Name = requestDto.Name;
            }

            if (requestDto.Phone != null)
            {
                user.Phone = requestDto.Phone;
            }

            if (requestDto.Image != null)
            {
                user.Photo = new List<Photo>
                {
                    new ()
                    {
                        ID = ObjectId.GenerateNewId().ToString(),
                        SmallImage = requestDto.Image[0],
                        MediumImage = requestDto.Image[1],
                        LargeImage = requestDto.Image[2]
                    }
                };
            }

            var updated = await UpdateOneAsync(user.ID, user);

            return updated;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<bool> DeActivateUser(string userId)
    {
        var user = await DB.Find<User>().OneAsync(userId);

        if (user == null)
        {
            return false;
        }

        if (!user.Active) return true;

        user.Active = false;
        var updated = await UpdateOneAsync(user.ID, user);

        return updated != null;
    }

    public Task<bool> ForgotPassword(string email)
    {
        return Task.Run(async () =>
        {
            var user = await FindUserByEmail(email);

            if (user == null)
            {
                return false;
            }

            user.PasswordResetToken = Guid.NewGuid().ToString();
            user.PasswordResetExpires = DateTime.Now.AddMinutes(10);

            var updated = await UpdateOneAsync(user.ID, user);
            return updated != null;
        });
    }

    public Task<User?> ResetPassword(string token, string password)
    {
        return Task.Run(async () =>
        {
            var user = await DB.Find<User>()
                .Match(p => p.PasswordResetToken == token)
                .Match(p => p.PasswordResetExpires > DateTime.Now)
                .ExecuteFirstAsync();

            if (user == null)
            {
                return null;
            }

            user.Password = HashPassword(password);
            user.PasswordResetToken = null;
            user.PasswordResetExpires = null;

            var updated = await UpdateOneAsync(user.ID, user);

            return updated;
        });
    }

    public Task<IEnumerable<User>> GetUsers(string? queryString)
    {
        return Task.Run(async () =>
        {
            if (queryString != null)
            {
                IMongoQueryable<User> query = DB.Collection<User>().AsQueryable();
                var features = new APIFeatures<User>(query, queryString);
                return await features.Filter().Sort().Paginate().ToListAsync();
            }

            IEnumerable<User> users = await DB.Find<User>()
                .ExecuteAsync();

            return users;
        });
    }

    public Task<User?> GetAdminById(string userId)
    {
        return Task.Run(async () =>
        {
            var user = await DB.Find<User>()
                .Match(p => p.ID == userId)
                .Match(p => p.Role == "admin")
                .ExecuteFirstAsync();

            return user;
        });
    }

    public Task<User?> GetUserByIdWithRelations(string userId)
    {
        return Task.Run(async () =>
        {
            var user = await FindById(userId);

            if (user == null)
            {
                return null;
            }

            user!.Wishlist = await DB.Find<Wishlist>()
                .Match(p => p.UserId == userId)
                .ExecuteAsync();

            foreach (var wishlist in user.Wishlist)
            {
                wishlist.Product = await DB.Find<Product>()
                    .Match(p => p.ID == wishlist.ProductId)
                    .ExecuteFirstAsync();
            }

            user!.Bag = await DB.Find<Card>()
                .Match(p => p.UserId == userId)
                .ExecuteAsync();

            foreach (var card in user.Bag)
            {
                card.Product = await DB.Find<Product>()
                    .Match(p => p.ID == card.ProductId)
                    .ExecuteFirstAsync();
            }

            return user;
        });
    }
}