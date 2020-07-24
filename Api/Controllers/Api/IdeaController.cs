using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Api.Abstracts;
using Models.Entities;

namespace Api.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class IdeaController : BasicCrudController<Project>
    {
        private readonly IProjectLogic _projectLogic;

        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="projectLogic"></param>
        /// <param name="userManager"></param>
        public IdeaController(IProjectLogic projectLogic, UserManager<User> userManager)
        {
            _projectLogic = projectLogic;
            _userManager = userManager;
        }

        protected override async Task<IBasicLogic<Project>> BasicLogic()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);

            return _projectLogic.For(user);
        }
    }
}