using System.Threading.Tasks;
using Logic.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels.Api;

namespace API.Controllers
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
    }
}