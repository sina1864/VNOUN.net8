using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;
using Vnoun.Core.Repositories.Dtos.Requests.Auth;

namespace Vnoun.Core.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> FindUserByEmail(string email);
    Task<User?> UpdateUserService(string userId, UpdateUserServiceRequestDto requestDto);
    Task<bool> DeActivateUser(string userId);
    Task<bool> ForgotPassword(string email);
    Task<User?> ResetPassword(string token, string password);
    Task<IEnumerable<User>> GetUsers(string? queryString);
    Task<User?> GetAdminById(string userId);
    Task<User?> GetUserByIdWithRelations(string userId);
}