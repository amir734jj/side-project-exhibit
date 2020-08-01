using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("[controller]")]
    public class ProjectsController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View();
        }
        
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            return View();
        }
        
        [HttpGet]
        [Route("Update")]
        public async Task<IActionResult> Update()
        {
            return View();
        }
    }
}