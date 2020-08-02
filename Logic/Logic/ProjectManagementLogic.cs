using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Entities;
using Models.Relationships;
using Models.ViewModels.Api;

namespace Logic.Logic
{
    public class ProjectManagementLogic : IProjectManagementLogic
    {
        private readonly IProjectLogic _projectLogic;
        private readonly ICategoryLogic _categoryLogic;

        public ProjectManagementLogic(IProjectLogic projectLogic, ICategoryLogic categoryLogic)
        {
            _projectLogic = projectLogic;
            _categoryLogic = categoryLogic;
        }
        
        public async Task Add(User user, ProjectViewModel projectViewModel)
        {
            var project = await _projectLogic.Save( new Project
            {
                User = user,
                Title = projectViewModel.Title,
                Description = projectViewModel.Description
            });

            project.ProjectCategoryRelationships = (await _categoryLogic.GetOrCreate(projectViewModel.Categories))
                .Select(x => new ProjectCategoryRelationship
                {
                    ProjectId = project.Id,
                    CategoryId = x.Id
                }).ToList();

            await _projectLogic.Update(project.Id, project);
        }

        public async Task Update(User user, ProjectViewModel projectViewModel)
        {
            var categories = (await _categoryLogic.GetOrCreate(projectViewModel.Categories))
                .Select(x => new ProjectCategoryRelationship
                {
                    ProjectId = projectViewModel.Id,
                    CategoryId = x.Id
                }).ToList();
            
            await _projectLogic.For(user).Update(projectViewModel.Id, project =>
            {
                project.Title = projectViewModel.Title;
                project.Description = projectViewModel.Description;
                project.ProjectCategoryRelationships = categories;
            });
        }

        public async Task Update(User user, int projectId)
        {
            await _projectLogic.For(user).Delete(projectId);
        }

        public async Task<ProjectViewModel> GetProjectById(User user, int projectId)
        {
            var project = await _projectLogic.For(user).Get(projectId);

            return new ProjectViewModel
            {
                Title = project.Title,
                Description = project.Description,
                Id = project.Id,
                Categories = project.ProjectCategoryRelationships.Select(x => x.Category.Name).ToList(),
                Votes = project.Votes
            };
        }
        
        public async Task<List<ProjectViewModel>> GetProjects(User user)
        {
            var projects = await _projectLogic.For(user).GetAll();

            return projects.Select(project =>
            {
                return new ProjectViewModel
                {
                    Title = project.Title,
                    Description = project.Description,
                    Id = project.Id,
                    Categories = project.ProjectCategoryRelationships.Select(x => x.Category.Name).ToList(),
                    Votes = project.Votes
                };
            }).ToList();
        }
    }
}