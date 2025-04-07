using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xpense.Core.Features.Analytics.UseCases;
using Xpense.RestApi.Helpers;
using Xpense.RestApi.Models;

namespace Xpense.RestApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/analytics")]
    public class AnalyticsController(
        GetExpensesByCategoryUseCase getExpensesByCategoryUseCase
        ) : XpenseController
    {
        [HttpGet("today/categories")]
        public async Task<IActionResult> GetExpensesByCategory()
        {
            var result = await getExpensesByCategoryUseCase.Execute();
            return Ok(TodayExpensesByCategoryResponse.Of(result));
        }
    }
}
