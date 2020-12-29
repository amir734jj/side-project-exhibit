using System.Linq;
using EfCoreRepository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal.Profiles
{
    public class CommentProfile : IEntityProfile<Comment>
    {
        public void Update(Comment entity, Comment dto)
        {
            entity.Text = dto.Text;
            entity.CreatedOn = dto.CreatedOn;
        }

        public IQueryable<Comment> Include<TQueryable>(TQueryable queryable) where TQueryable : IQueryable<Comment>
        {
            return queryable
                .Include(x => x.Project)
                .ThenInclude(x => x.User)
                .Include(x => x.User);
        }
    }
}