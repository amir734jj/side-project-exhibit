using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        [HttpGet]
        [Route("404")]
        public IActionResult Error404()
        {
            return View("404");
        }
    }
}