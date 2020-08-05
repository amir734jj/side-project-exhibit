using System.Threading.Tasks;
using Models.Entities;
using Models.Enums;
using Models.ViewModels.Api;

namespace Logic.Interfaces
{
    public interface IBoardLogic
    {
        Task<BoardViewModels> Collect(int page, Sort sort, Order order, int pageSize, string category, string keyword);

        Task<Project> Vote(int ideaId, User user, Vote vote);
        
        Task<Project> AddComment(int projectId, User user, CommentViewModel comment);
        
        Task<Project> DeleteComment(int projectId, User user, int commentId);
        
        Task<Project> EditComment(int projectId, User userId, int commentId, CommentViewModel comment);
    }
}