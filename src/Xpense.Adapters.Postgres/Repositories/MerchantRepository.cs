using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Models;

namespace Xpense.Persistence.Repositories
{
    public class MerchantRepository(XpenseDbContext context) : OptionRepository<Merchant>(context), IMerchantRepository
    {

    }
}
