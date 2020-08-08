using System;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Models.Enums;

namespace API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "Admin")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IEfRepository _efRepository;
        
        private readonly IUserLogic _userLogic;
        
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="userLogic"></param>
        /// <param name="userManager"></param>
        public UserController(IEfRepository efRepository, IUserLogic userLogic, UserManager<User> userManager)
        {
            _efRepository = efRepository;
            _userLogic = userLogic;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns user view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            return View(await _userLogic.GetAll());
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userLogic.Delete(id);

            return RedirectToAction("Index");
        }
         
        /// <summary>
        /// Update User Role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("UpdateUserRole/{id}/{userRoleEnum}")]
        public async Task<IActionResult> UpdateUserRole(int id, UserRoleEnum userRoleEnum)
        {
            var userLogicEf = _efRepository.For<User, int>().Session();

            var user = await userLogicEf.Get(id);

            user.UserRole = userRoleEnum;
            
            await userLogicEf.Update(id, user);

            switch (userRoleEnum)
            {
                case UserRoleEnum.Basic:
                    await _userManager.AddToRoleAsync(user, UserRoleEnum.Basic.ToString());
                    await _userManager.RemoveFromRoleAsync(user, UserRoleEnum.Admin.ToString());
                    break;
                case UserRoleEnum.Admin:
                    await _userManager.AddToRoleAsync(user, UserRoleEnum.Admin.ToString());
                    await _userManager.AddToRoleAsync(user, UserRoleEnum.Basic.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(userRoleEnum), userRoleEnum, null);
            }
            
            return RedirectToAction("Index");
        }
    }
}