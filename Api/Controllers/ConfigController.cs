using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Config;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class ConfigController : Controller
    {
        private readonly IConfigLogic _configLogic;

        public ConfigController(IConfigLogic configLogic)
        {
            _configLogic = configLogic;
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            var result = _configLogic.ResolveGlobalConfig();
            
            return View(result);
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> UpdateConfig(GlobalConfigViewModel globalConfigViewModel)
        {
            await _configLogic.UpdateGlobalConfig(globalConfigViewModel);

            return RedirectToAction("Index");
        }
    }
}