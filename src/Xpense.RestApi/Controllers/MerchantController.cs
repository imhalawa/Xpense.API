using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Xpense.Core.Features.Merchants.UseCases;
using Xpense.RestApi.Helpers;
using Xpense.RestApi.Models;

namespace Xpense.RestApi.Controllers
{
    [Route("api/merchant")]
    [ApiController]
    public class MerchantController(
        GetAllMerchantsUseCase getAllMerchantsUse
    ) : XpenseController
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var merchants = await getAllMerchantsUse.Execute();
            return Ok(merchants.Select(MerchantResponse.Of));
        }
    }
}
