using System.Threading.Tasks;
using Logic.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels.Api;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("Projects")]
    public class ProjectManagementController : Controller
    {
        private readonly IProjectManagementLogic _projectManagementLogic;
        
        private readonly UserManager<User> _userManager;

        public ProjectManagementController(IProjectManagementLogic projectManagementLogic,
            UserManager<User> userManager)
        {
            _projectManagementLogic = projectManagementLogic;
            _userManager = userManager;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var user = await GetUser();

            var projects = await _projectManagementLogic.GetProjects(user);
            
            TempData.CopyTo(ViewData);

            return View(projects);
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
            if (!ModelState.IsValid)
            {
                return BadRequest("ModelState validation failed");
            }
            
            var user = await GetUser();

            await _projectManagementLogic.Add(user, projectViewModel);

            return Ok();
        }

        [HttpGet]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id)
        {
            return View();
        }

        [HttpGet]
        [Route("{id}/json")]
        public async Task<IActionResult> GetProjectBy(int id)
        {
            var user = await GetUser();

            var project = await _projectManagementLogic.GetProjectById(user, id);

            return Ok(project);
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> UpdateHandler([FromBody] ProjectViewModel projectViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("ModelState validation failed");
            }

            var user = await GetUser();

            await _projectManagementLogic.Update(user, projectViewModel);

            return Ok();
        }

        private async Task<User> GetUser()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return user;
        }
        
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await _projectManagementLogic.Delete(user, id);

            TempData["Message"] = "Successfully deleted the project";

            return RedirectToAction("Index");
        }
    }
}