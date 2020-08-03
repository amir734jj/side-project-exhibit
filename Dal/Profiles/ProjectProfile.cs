using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Profiles
{
    public class ProjectProfile : IEntityProfile<Project, int>
    {
        private readonly IEntityProfileAuxiliary _entityProfileAuxiliary;

        public ProjectProfile(IEntityProfileAuxiliary entityProfileAuxiliary)
        {
            _entityProfileAuxiliary = entityProfileAuxiliary;
        }
        
        public Project Update(Project entity, Project dto)
        {
            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.CreatedOn = dto.CreatedOn;
            entity.Votes = dto.Votes;
            entity.ProjectCategoryRelationships = _entityProfileAuxiliary.ModifyList(entity.ProjectCategoryRelationships, dto.ProjectCategoryRelationships, x => new { x.ProjectId, x.CategoryId});
            entity.Comments = _entityProfileAuxiliary.ModifyList<Comment, int>(entity.Comments, dto.Comments);

            return entity;
        }

        public IQueryable<Project> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Project>
        {
            return queryable
                .Include(x => x.User)
                .Include(x => x.Votes)
                .Include(x => x.ProjectCategoryRelationships)
                .ThenInclude(x => x.Category)
                .ThenInclude(x => x.ProjectCategoryRelationships)
                .Include(x => x.ProjectCategoryRelationships)
                .ThenInclude(x => x.Project)
                .Include(x => x.Comments);
        }
    }
}