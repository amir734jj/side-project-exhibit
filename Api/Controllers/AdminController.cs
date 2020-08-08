using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.Api;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly IAdminLogic _adminLogic;
        
        private readonly ICategoryLogic _categoryLogic;

        public AdminController(IAdminLogic adminLogic, ICategoryLogic categoryLogic)
        {
            _adminLogic = adminLogic;
            _categoryLogic = categoryLogic;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            var panel = await _adminLogic.GetPanel();
            
            return View(panel);
        }
        
        [HttpGet]
        [Route("Category/Clean")]
        public async Task<IActionResult> CleanOrphans()
        {
            await _categoryLogic.CleanOrphans();
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Comment/{id}/Delete")]
        public async Task<IActionResult> CommentDelete([FromRoute] int id)
        {
            await _adminLogic.DeleteComment(id);
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Project/{id}/Delete")]
        public async Task<IActionResult> ProjectDelete([FromRoute] int id)
        {
            await _adminLogic.DeleteProject(id);
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Comment/{id}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id)
        {
            var comment = await _adminLogic.GetComment(id);
            
            return View(comment);
        }
        
        [HttpPost]
        [Route("Comment/{id}")]
        public async Task<IActionResult> UpdateCommentHandler([FromRoute] int id, CommentViewModel commentViewModel)
        {
            await _adminLogic.UpdateComment(id, commentViewModel);
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("Project/{id}")]
        public async Task<IActionResult> UpdateProject([FromRoute] int id)
        {
            return View();
        }
        
        [HttpGet]
        [Route("Project/{id}/json")]
        public async Task<IActionResult> UpdateProjectJson([FromRoute] int id)
        {
            var project = await _adminLogic.GetProject(id);

            return Ok(project);
        }
        
        [HttpPut]
        [Route("Project/{id}")]
        public async Task<IActionResult> UpdateProjectHandler([FromRoute] int id, [FromBody] ProjectViewModel projectViewModel)
        {
            await _adminLogic.UpdateProject(id, projectViewModel);

            return Ok();
        }
    }
}