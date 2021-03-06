﻿using System.Linq;
using System.Threading.Tasks;
using Api.Abstracts;
using Logic.Extensions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        
        private readonly IUserLogic _userLogic;

        private readonly IUserSetup _userSetup;

        public AccountController(IUserSetup userSetup, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager, IUserLogic userLogic)
        {
            _userSetup = userSetup;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userLogic = userLogic;
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

        public override IUserLogic UserLogic()
        {
            return _userLogic;
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
        
        [HttpGet]
        [Route("Setup/{userId}")]
        [SwaggerOperation("Setup")]
        [AllowAnonymous]
        public async Task<IActionResult> Setup([FromRoute] int userId)
        {
            await _userSetup.Setup(userId);

            return RedirectToAction("Login");
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

            if (result.ReturnValue.Item1)
            {
                return RedirectToAction("Setup", new {userId = result.ReturnValue.Item2.Id});
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