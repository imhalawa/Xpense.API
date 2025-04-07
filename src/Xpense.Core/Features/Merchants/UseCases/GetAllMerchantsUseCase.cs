using Xpense.Core.Abstract.Persistence;
using Xpense.Core.Abstract.UseCases;
using Xpense.Core.Entities;

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
