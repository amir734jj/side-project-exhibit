using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;

namespace Api.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        private readonly INotificationLogic _notificationLogic;
        
        private readonly UserManager<User> _userManager;

        public NotificationController(INotificationLogic notificationLogic, UserManager<User> userManager)
        {
            _notificationLogic = notificationLogic;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Collect()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var result = await _notificationLogic.Collect(user);

            return Ok(result);
        }
        
        [HttpPost]
        [Route("MarkAsSeen")]
        public async Task<IActionResult> MarkAsSeen()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            await _notificationLogic.MarkAsSeen(user);

            return Ok();
        }
    }
}