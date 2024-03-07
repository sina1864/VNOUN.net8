using Vnoun.Core.Entities;
using Vnoun.Core.Repositories.Base;

namespace Vnoun.Core.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task DeleteFilterSectionAsync(string categoryId, string filterId);
    Task DeleteAllCategories();
}