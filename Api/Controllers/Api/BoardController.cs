using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Models.Entities;
using Models.Enums;

namespace Api.Controllers.Api
{
    // [ApiExplorerSettings(IgnoreApi = true)]
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
        public async Task<IActionResult> Index([FromRoute] int index = 0, [FromQuery] Sort sort = Sort.Vote,
            [FromQuery] Order order = Order.Descending,  [FromQuery]int pageSize = 10,
            [FromQuery] string category = "", [FromQuery] string keyword = "")
        {
            var board = await _boardLogic.Collect(index, sort, order, pageSize, category, keyword);

            return Ok(board);
        }

        [Authorize]
        [Route("vote/{id}/{vote}")]
        [HttpPost]
        public async Task<IActionResult> Vote([FromRoute] int id, [FromRoute] Vote vote)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            var project = await _boardLogic.Vote(id, user.Id, vote);

            return Ok(project);
        }
    }
}