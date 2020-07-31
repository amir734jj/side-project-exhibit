using System.Collections;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Abstracts;
using Models.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers.Api
{
    // [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("api/[controller]")]
    public class CategoryController : BasicCrudController<Category>
    {
        private readonly ICategoryLogic _categoryLogic;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="categoryLogic"></param>
        public CategoryController(ICategoryLogic categoryLogic)
        {
            _categoryLogic = categoryLogic;
        }

        [NonAction]
        protected override async Task<IBasicLogic<Category>> BasicLogic()
        {
            return _categoryLogic;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("")]
        [SwaggerOperation("GetAll")]
        [ProducesResponseType(typeof(IEnumerable), 200)]
        public override Task<IActionResult> GetAll()
        {
            return base.GetAll();
        }
    }
}