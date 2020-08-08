using System.Threading.Tasks;
using Models.ViewModels.Api;

namespace Logic.Interfaces
{
    public interface IAdminLogic
    {
        Task<AdminPanel> GetPanel();

        Task<CommentViewModel> GetComment(int id);

        Task<ProjectViewModel> GetProject(int id);

        Task DeleteComment(int id);
        
        Task DeleteProject(int id);
        
        Task UpdateComment(int id, CommentViewModel commentViewModel);
        
        Task UpdateProject(int id, ProjectViewModel projectViewModel);
    }
}