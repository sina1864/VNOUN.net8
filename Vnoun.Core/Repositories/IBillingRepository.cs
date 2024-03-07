using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;
using Vnoun.Core.Repositories.Dtos.Responses.billings;

namespace Vnoun.Core.Repositories;

public interface IBillingRepository : IRepository<Billing>
{
    Task<MyStatsResultDto> GetMyStatsAsync(string userId);
    Task<AdminStatsResultDto> GetAdminStatsAsync();
    Task<List<Billing>> GetMyBillingsAsync(string userId, string? queryString);
    Task<List<Billing>> GetBillingsWithIds(List<string> ids);
}