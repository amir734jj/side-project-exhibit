using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Profiles
{
    public class UserProfile : IEntityProfile<User>
    {
        public void Update(User entity, User dto)
        {
            entity.LastLoginTime = dto.LastLoginTime;
            entity.UserRole = dto.UserRole;
            entity.UserNotifications = dto.UserNotifications;
        }

        public IQueryable<User> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<User>
        {
            return queryable
                .Include(x => x.Votes)
                .ThenInclude(x => x.User)
                .Include(x => x.Votes)
                .ThenInclude(x => x.Project)
                .Include(x => x.Comments)
                .Include(x => x.Projects)
                .ThenInclude(x => x.ProjectCategoryRelationships)
                .Include(x => x.Projects)
                .ThenInclude(x => x.Votes);
        }
    }
}