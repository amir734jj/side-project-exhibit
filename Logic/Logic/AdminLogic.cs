using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Crud;
using Logic.Interfaces;
using Models.Relationships;
using Models.ViewModels.Api;

namespace Logic.Logic
{
    public class AdminLogic : IAdminLogic
    {
        private readonly IProjectLogic _projectLogic;
        
        private readonly ICommentLogic _commentLogic;
        
        private readonly ICategoryLogic _categoryLogic;
        
        private readonly IEfRepository _efRepository;

        public AdminLogic(IProjectLogic projectLogic, ICommentLogic commentLogic, ICategoryLogic categoryLogic, IEfRepository efRepository)
        {
            _projectLogic = projectLogic;
            _commentLogic = commentLogic;
            _categoryLogic = categoryLogic;
            _efRepository = efRepository;
        }
        
        public async Task<AdminPanel> GetPanel()
        {
            return new AdminPanel
            {
                Projects = (await _projectLogic.GetAll()).ToList(),
                Comments = (await _commentLogic.GetAll()).ToList(),
                Categories = (await _categoryLogic.GetAll()).ToList()
            };
        }

        public async Task<CommentViewModel> GetComment(int id)
        {
            var comment = await _commentLogic.Get(id);

            return new CommentViewModel {Comment = comment.Text};
        }

        public async Task<ProjectViewModel> GetProject(int id)
        {
            var project = await _projectLogic.Get(id);

            return new ProjectViewModel
            {
                Title = project.Title,
                Description = project.Description,
                Categories = project.ProjectCategoryRelationships.Select(x => x.Category.Name).ToList(),
            };
        }

        public async Task DeleteComment(int id)
        {
            await _commentLogic.Delete(id);
        }

        public async Task DeleteProject(int id)
        {
            await _projectLogic.Delete(id);
        }

        public async Task UpdateComment(int id, CommentViewModel commentViewModel)
        {
            await _commentLogic.Update(id, comment =>
            {
                comment.Text = comment.Text;
            });
        }

        public async Task UpdateProject(int id, ProjectViewModel projectViewModel)
        {
            var disposableResult = await _categoryLogic.SetRepository(_efRepository).GetOrCreate(projectViewModel.Categories);
            
            var categories = disposableResult
                .Select(x => new ProjectCategoryRelationship
                {
                    ProjectId = projectViewModel.Id,
                    CategoryId = x.Id
                }).ToList();
            
            await _projectLogic.SetRepository(_efRepository).Update(projectViewModel.Id, project =>
            {
                project.Title = projectViewModel.Title;
                project.Description = projectViewModel.Description;
                project.ProjectCategoryRelationships = categories;
            });
        }
    }
}