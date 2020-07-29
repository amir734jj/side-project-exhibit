using System.Linq;
using EfCoreRepository.Interfaces;
using Models.Entities;

namespace Dal.Profiles
{
    public class CategoryProfile : IEntityProfile<Category, int>
    {
        public Category Update(Category entity, Category dto)
        {
            entity.Name = dto.Name;

            return entity;
        }

        public IQueryable<Category> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Category>
        {
            return queryable;
        }
    }
}