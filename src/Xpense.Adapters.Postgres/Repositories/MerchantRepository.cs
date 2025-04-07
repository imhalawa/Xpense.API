using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Entities;

namespace Xpense.Persistence.Repositories
{
    public class MerchantRepository(XpenseDbContext context) : OptionRepository<Merchant>(context), IMerchantRepository
    {

    }
}
