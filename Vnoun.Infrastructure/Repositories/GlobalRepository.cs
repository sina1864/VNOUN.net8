using MongoDB.Driver;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;
using Vnoun.Infrastructure.Repositories.Base;

namespace Vnoun.Infrastructure.Repositories;

public class GlobalRepository : Repository<Global>, IGlobalRepository
{
    public async Task<Global?> GetGlobalSettings()
    {
        var all = await GetAllAsync();

        return all.FirstOrDefault();
    }
}