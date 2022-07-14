using Microsoft.AspNetCore.Mvc;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Services;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ExpressionsController : Controller
    {
        private readonly SecullumInternationalizationWebServiceContext _secullumInternationalizationWebServiceContext;

        public ExpressionsController(SecullumInternationalizationWebServiceContext secullumInternationalizationWebServiceContext)
        {
            _secullumInternationalizationWebServiceContext = secullumInternationalizationWebServiceContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ExpressionsService.GenerateParameters parameters)
        {
            var service = new ExpressionsService(_secullumInternationalizationWebServiceContext);

            try
            {
                return Json(await service.GenerateAsync(parameters));
            }
            catch (ExpressionsService.GenerateException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }
    }
}
