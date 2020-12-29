using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("User")]
    public class PublicProfileController : Controller
    {
        private readonly IUserLogic _userLogic;

        public PublicProfileController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userLogic.Get(id);

            return View(user);
        }

        [HttpGet]
        [Route("in/{username}")]
        public async Task<IActionResult> IndexByUsername(string username)
        {
            var user = (await _userLogic.GetWhere(x => x.UserName == username)).FirstOrDefault();

            return user != null ? RedirectToAction("Index", "PublicProfile", new {id = user.Id}) : RedirectToAction("Error404", "Error");
        }
    }
}