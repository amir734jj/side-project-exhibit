using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels.Api;

namespace Logic.Interfaces
{
    public interface IProjectManagementLogic
    {
        Task Add(User user, ProjectViewModel projectViewModel);
        
        Task Update(User user, ProjectViewModel projectViewModel);
        
        Task Update(User user, int projectId);
        
        Task<ProjectViewModel> GetProjectById(User user, int projectId);

        Task<List<ProjectViewModel>> GetProjects(User user);
    }
}