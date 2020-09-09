using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly IProjectLogic _projectLogic;

        public HomeController(IProjectLogic projectLogic)
        {
            _projectLogic = projectLogic;
        }

        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Project/{id}")]
        public async Task<IActionResult> Project(int id)
        {
            var project = await _projectLogic.Get(id);
            
            return View(project);
        }

        [HttpGet]
        [Route("03a01a77d19c0563a6c59f694cbe0d07.txt")]
        public IActionResult Temp()
        {
            return Ok("");
        }
    }
}