using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Services;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ExpressionsController : Controller
    {
        private readonly ExpressionsService _expressionsService;

        public ExpressionsController(SecullumInternationalizationWebServiceContext seci18nWebServiceContext, IOptions<TranslatorSettings> settings)
        {
            _expressionsService = new ExpressionsService(seci18nWebServiceContext, settings);
        }
                
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ExpressionsService.GenerateParameters parameters)
        {
            try
            {
                return Json(await _expressionsService.GenerateAsync(parameters));
            }
            catch (ExpressionsService.GenerateException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }
    }
}
