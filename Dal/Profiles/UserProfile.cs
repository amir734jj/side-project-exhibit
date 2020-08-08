using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Profiles
{
    public class UserProfile : IEntityProfile<User, int>
    {
        public User Update(User entity, User dto)
        {
            entity.LastLoginTime = dto.LastLoginTime;
            entity.UserRole = dto.UserRole;

            return entity;
        }

        public IQueryable<User> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<User>
        {
            return queryable
                .Include(x => x.Votes)
                .Include(x => x.Comments)
                .Include(x => x.Projects)
                .ThenInclude(x => x.ProjectCategoryRelationships)
                .Include(x => x.Projects)
                .ThenInclude(x => x.Votes);
        }
    }
}