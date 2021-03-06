using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Profiles
{
    public class CategoryProfile : IEntityProfile<Category>
    {
        private readonly IEntityProfileAuxiliary _entityProfileAuxiliary;

        public CategoryProfile(IEntityProfileAuxiliary entityProfileAuxiliary)
        {
            _entityProfileAuxiliary = entityProfileAuxiliary;
        }

        public void Update(Category entity, Category dto)
        {
            entity.Name = dto.Name;
            entity.ProjectCategoryRelationships =  _entityProfileAuxiliary.ModifyList(entity.ProjectCategoryRelationships, dto.ProjectCategoryRelationships, x => new { x.CategoryId, x.ProjectId});
        }

        public IQueryable<Category> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Category>
        {
            return queryable
                .Include(x => x.ProjectCategoryRelationships)
                .ThenInclude(x => x.Project);
        }
    }
}