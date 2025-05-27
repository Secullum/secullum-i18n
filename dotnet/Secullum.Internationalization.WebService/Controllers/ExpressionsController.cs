using Microsoft.AspNetCore.Mvc;
using Secullum.Internationalization.WebService.Services;
using System.Threading.Tasks;

namespace Secullum.Internationalization.WebService.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ExpressionsController : Controller
    {
        private readonly ExpressionsService _expressionsService;

        public ExpressionsController(ExpressionsService expressionsService)
        {
            _expressionsService = expressionsService;
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

        [HttpPost("TranslateAll")]
        public async Task<IActionResult> TranslateAllAsync()
        {
            try
            {
                return Json(await _expressionsService.TranslateAllExpressionsAsync());
            }
            catch (ExpressionsService.GenerateException ex)
            {
                return BadRequest(new { errorMessage = ex.Message });
            }
        }
    }
}
