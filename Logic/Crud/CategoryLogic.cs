using EfCoreRepository.Interfaces;
using Logic.Abstracts;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Crud
{
    public class CategoryLogic : BasicLogicAbstract<Category>, ICategoryLogic
    {
        private readonly IBasicCrudType<Category, int> _categoryDal;

        /// <summary>
        /// Constructor dependency injection
        /// </summary>
        /// <param name="repository"></param>
        public CategoryLogic(IEfRepository repository)
        {
            _categoryDal = repository.For<Category, int>();
        }

        /// <summary>
        /// Returns DAL
        /// </summary>
        /// <returns></returns>
        protected override IBasicCrudType<Category, int> GetBasicCrudDal()
        {
            return _categoryDal;
        }
    }
}