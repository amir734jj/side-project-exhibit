using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<List<Category>> GetOrCreate(List<string> items)
        {
            var logic = _categoryDal;
            
            var categories = (await logic.GetAll()).ToList();

            var joinedResult = items
                .GroupJoin(categories, c => c.ToLower(), p => p.Name, (c, ps) => new {c, ps})
                .SelectMany(t => t.ps.DefaultIfEmpty(), (t, p) => (t.c, p)).ToList();

            await Task.WhenAll(
                joinedResult.Where(x => x.p == null)
                    .Select(x => x.c)
                    .Select(x => logic.Save(new Category {Name = x.ToLower()})));
            
            categories = (await logic.GetAll()).ToList();

            return categories.Join(items, x => x.Name.ToLower(), x => x, (category, s) => category).ToList();
        }

        public ICategoryLogic SetRepository(IEfRepository efRepository)
        {
            return new CategoryLogic(efRepository);
        }

        public async Task CleanOrphans()
        {
            await using var logic = _categoryDal.Session();

            foreach (var category in await logic.GetAll())
            {
                if (category.ProjectCategoryRelationships == null ||
                    category.ProjectCategoryRelationships.All(y => y.Project == null))
                {
                    await logic.Delete(category.Id);
                }
            }
        }
    }
}