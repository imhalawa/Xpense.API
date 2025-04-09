using Xpense.Core.Models;

namespace Xpense.Core.Abstract.Persistence;

public interface ICategoryRepository: IRepository<Category>
{
    Task DeleteById(int id);
    Task<bool> Exists(int id);
}