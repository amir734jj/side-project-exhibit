using System.Linq;
using EfCoreRepository.Interfaces;
using Models.Entities;

namespace Dal.Profiles
{
    public class CategoryProfile : IEntityProfile<Category, int>
    {
        private readonly IEntityProfileAuxiliary _entityProfileAuxiliary;

        public CategoryProfile(IEntityProfileAuxiliary entityProfileAuxiliary)
        {
            _entityProfileAuxiliary = entityProfileAuxiliary;
        }

        public Category Update(Category entity, Category dto)
        {
            entity.Name = dto.Name;
            entity.ProjectCategoryRelationships =  _entityProfileAuxiliary.ModifyList(entity.ProjectCategoryRelationships, dto.ProjectCategoryRelationships, x => new { x.CategoryId, x.ProjectId});

            return entity;
        }

        public IQueryable<Category> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Category>
        {
            return queryable;
        }
    }
}