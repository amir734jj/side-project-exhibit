using System.Linq;
using System.Threading.Tasks;
using Api.Abstracts;
using Logic.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.ViewModels.Identities;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]")]
    public class AccountController : AbstractAccountController
    {        
        private readonly UserManager<User> _userManager;
        
        private readonly SignInManager<User> _signInManager;
        
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public override UserManager<User> ResolveUserManager()
        {
            return _userManager;
        }

        public override SignInManager<User> ResolveSignInManager()
        {
            return _signInManager;
        }

        public override RoleManager<IdentityRole<int>> ResolveRoleManager()
        {
            return _roleManager;
        }

        /// <summary>
        ///     View page to login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Login")]
        [SwaggerOperation("Login")]
        public async Task<IActionResult> Login()
        {
            TempData.CopyTo(ViewData, "ErrorKey", "ErrorValues", "Message");

            return View();
        }
        
        /// <summary>
        ///     Handles login the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("LoginHandler")]
        [SwaggerOperation("LoginHandler")]
        public async Task<IActionResult> LoginHandler(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorKey"] = "Model state validation failed";
                TempData["ErrorValues"] = string.Join("\n", ModelState.Values.SelectMany(x => x.Errors.Select(y =>y.ErrorMessage)));

                return RedirectToAction("Login");
            }
            
            var result = await base.Login(loginViewModel);

            if (result.ReturnValue)
            {
                return Redirect(Url.Content("~/"));
            }

            TempData["ErrorKey"] = "Failed to login";
            TempData["ErrorValues"] = string.Join("\n", result.Errors);

            return RedirectToAction("Login");
        }
        
        /// <summary>
        ///     View page to register
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Register")]
        [SwaggerOperation("Register")]
        public async Task<IActionResult> Register()
        {
            TempData.CopyTo(ViewData, "ErrorKey", "ErrorValues", "Message");
            
            return View();
        }
        
        /// <summary>
        ///     Register the user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RegisterHandler")]
        [SwaggerOperation("RegisterHandler")]
        public async Task<IActionResult> RegisterHandler(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorKey"] = "Model state validation failed";
                TempData["ErrorValues"] = string.Join("\n", ModelState.Values.Select(x => x.Errors.Select(y =>y.ErrorMessage)));

                return RedirectToAction("Register");
            }

            // Save the user
            var result = await Register(registerViewModel);

            if (result.ReturnValue)
            {
                TempData["Message"] = "Successfully registered";

                return RedirectToAction("Register");
            }

            TempData["ErrorKey"] = "Failed to register";
            TempData["ErrorValues"] = string.Join("\n", result.Errors);

            return RedirectToAction("Register");
        }
        
        /// <summary>
        ///     Not authenticated view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("NotAuthenticated")]
        [SwaggerOperation("NotAuthenticated")]
        public async Task<IActionResult> NotAuthenticated()
        {
            return View();
        }

        /// <summary>
        ///     Not authorized view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Logout")]
        [SwaggerOperation("Logout")]
        public async Task<IActionResult> Logout()
        {
            await base.Logout();

            return RedirectToAction("Login");
        }
    }
}