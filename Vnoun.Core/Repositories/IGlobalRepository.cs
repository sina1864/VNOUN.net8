using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface IGlobalRepository : IRepository<Global>
{
    Task<Global?> GetGlobalSettings();
}