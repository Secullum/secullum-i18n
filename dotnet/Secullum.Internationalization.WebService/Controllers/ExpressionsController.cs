using Microsoft.AspNetCore.Mvc;
using Secullum.Internationalization.WebService.Data;
using Secullum.Internationalization.WebService.Services;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Controllers
{
    [ApiController]
    public class ExpressionsController : Controller
    {
        private readonly SecullumInternationalizationWebServiceContext _secullumInternationalizationWebServiceContext;
        public ExpressionsController(SecullumInternationalizationWebServiceContext secullumInternationalizationWebServiceContext)
        {
            _secullumInternationalizationWebServiceContext = secullumInternationalizationWebServiceContext;
        }
        [Route("/[controller]")]
        [HttpPost]
        public async Task<IActionResult> ExpressionPost([FromBody] TranslatorService.ParametersExpressionsList parameters)
        {
            var translatorService = new TranslatorService(_secullumInternationalizationWebServiceContext);
            var errorMessage = translatorService.ErrorStringReturn(parameters);
            if (errorMessage != null)
            {
                return BadRequest(new { errorMessage = errorMessage });
            }

            return Json(await translatorService.TranslatedExpressionsReturnAsync(parameters));
        }
    }
}