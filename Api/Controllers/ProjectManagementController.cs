using System.Threading.Tasks;
using Logic.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Api;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [Route("Projects")]
    public class ProjectManagementController : Controller
    {
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            TempData.CopyTo(ViewData, "ErrorKey", "ErrorValues", "Message");
            
            return View();
        }
        
        [HttpGet]
        [Route("Add")]
        public async Task<IActionResult> Add()
        {
            return View();
        }
        
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddHandler([FromBody] ProjectViewModel projectViewModel)
        {
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            return View();
        }
        
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> UpdateHandler([FromBody] ProjectViewModel projectViewModel)
        {
            return RedirectToAction("Index");
        }
    }
}