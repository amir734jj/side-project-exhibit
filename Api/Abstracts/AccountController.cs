using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Basics;
using Models.Entities;
using Models.Enums;
using Models.ViewModels.Identities;

namespace Api.Abstracts
{
    public abstract class AbstractAccountController : Controller
    {
        public abstract UserManager<User> ResolveUserManager();

        public abstract SignInManager<User> ResolveSignInManager();

        public abstract RoleManager<IdentityRole<int>> ResolveRoleManager();

        public abstract IUserLogic UserLogic();

        public async Task<ReturnWithErrors<bool>> Register(RegisterViewModel registerViewModel)
        {
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                return new ReturnWithErrors<bool>(false, new List<string> { "Passwords do not match"});
            }
            
            var role = ResolveUserManager().Users.Any() ? UserRoleEnum.Basic : UserRoleEnum.Admin;

            var user = new User
            {
                Name = registerViewModel.Name,
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserRole = role
            };

            var result1 = await ResolveUserManager().CreateAsync(user, registerViewModel.Password);

            if (!result1.Succeeded)
            {
                return new ReturnWithErrors<bool>(false,result1.Errors.Select(err => err.Description).ToList());
            }

            var result2 = new List<IdentityResult>();

            foreach (var subRole in role.SubRoles())
            {
                if (!await ResolveRoleManager().RoleExistsAsync(subRole.ToString()))
                {
                    await ResolveRoleManager().CreateAsync(new IdentityRole<int>(subRole.ToString()));
                    
                    result2.Add(await ResolveUserManager().AddToRoleAsync(user, subRole.ToString()));
                }
            }

            return new ReturnWithErrors<bool>(result2.All(x => x.Succeeded),
                result2.SelectMany(errs => errs.Errors.Select(err => err.Description)).ToList());
        }

        public async Task<ReturnWithErrors<bool>> Login(LoginViewModel loginViewModel)
        {
            // Ensure the username and password is valid.
            var result = await ResolveUserManager().FindByNameAsync(loginViewModel.Username);

            if (result == null || !await ResolveUserManager().CheckPasswordAsync(result, loginViewModel.Password))
            {
                return new ReturnWithErrors<bool>(false,
                    new List<string> {"Username/Password combination is not valid"});
            }

            await ResolveSignInManager().SignInAsync(result, true);

            // Generate and issue a JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.Email)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.Now.AddDays(1),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(principal), authProperties);
            
            await UserLogic().Update(result.Id, x => x.LastLoginTime = DateTimeOffset.Now);

            return new ReturnWithErrors<bool>(true);
        }

        public async Task Logout()
        {
            await ResolveSignInManager().SignOutAsync();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}