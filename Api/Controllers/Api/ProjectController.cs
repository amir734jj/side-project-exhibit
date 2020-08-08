using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Api.Abstracts;
using Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Api
{
    // [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("api/[controller]")]
    public class ProjectController : BasicCrudController<Project>
    {
        private readonly IProjectLogic _projectLogic;

        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="projectLogic"></param>
        /// <param name="userManager"></param>
        public ProjectController(IProjectLogic projectLogic, UserManager<User> userManager)
        {
            _projectLogic = projectLogic;
            _userManager = userManager;
        }

        [NonAction]
        protected override async Task<IBasicLogic<Project>> BasicLogic()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            return _projectLogic.For(user);
        }
    }
}