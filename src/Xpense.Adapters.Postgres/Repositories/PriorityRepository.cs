using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Entities;

namespace Xpense.Persistence.Repositories
{
    public class PriorityRepository(XpenseDbContext dbContext) : Repository<Priority>(dbContext), IPriorityRepository
    {
    }
}
