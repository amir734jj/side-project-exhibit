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
    [Authorize]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly IProfileLogic _profileLogic;

        public ProfileController(UserManager<User> userManager, IProfileLogic profileLogic)
        {
            _userManager = userManager;
            _profileLogic = profileLogic;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            TempData.CopyTo(ViewData, "ErrorKey", "ErrorValues", "Message");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var profile = new ProfileViewModel(user);

            return View(profile);
        }
        
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Update(ProfileViewModel profileViewModel)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await _profileLogic.Update(user, profileViewModel);

            TempData["Message"] = "Successfully updated profile";
            
            return RedirectToAction("Index");
        }
    }
}