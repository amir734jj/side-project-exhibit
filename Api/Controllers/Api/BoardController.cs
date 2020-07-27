using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Entities;
using Models.Enums;

namespace Api.Controllers.Api
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class BoardController : Controller
    {
        private readonly IBoardLogic _boardLogic;
        
        private readonly UserManager<User> _userManager;

        public BoardController(IBoardLogic boardLogic, UserManager<User> userManager)
        {
            _boardLogic = boardLogic;
            _userManager = userManager;
        }

        [Route("{page?}")]
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute] int page = 1, [FromQuery] Sort sort = Sort.Vote, [FromQuery] Order order = Order.Descending)
        {
            var ideas = await _boardLogic.Collect(page, sort, order);

            return Ok(ideas);
        }
        
        [Authorize]
        [Route("vote/{id}/{vote}")]
        [HttpGet]
        public async Task<IActionResult> Vote([FromRoute] int id, [FromRoute] Vote vote)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            
            var project = await _boardLogic.Vote(id, user.Id, vote);

            return Ok(project);
        }
    }
}