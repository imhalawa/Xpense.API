using Xpense.Core.Interfaces.Persistence;
using Xpense.Core.Interfaces.UseCases;
using Xpense.Core.Models;

namespace Xpense.Core.Features.Merchants.UseCases
{
    public class GetAllMerchantsUseCase(IMerchantRepository repository) : IQueryHandler<IEnumerable<Merchant>>
    {
        public async Task<IEnumerable<Merchant>> Execute()
        {
            var result = await repository.GetAll();
            return result;
        }
    }
}
