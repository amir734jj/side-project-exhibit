using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCoreRepository.Interfaces;
using Logic.Interfaces;
using Models.Entities;
using Models.Relationships;
using Models.ViewModels.Api;

namespace Logic.Logic
{
    public class ProjectManagementLogic : IProjectManagementLogic
    {
        private readonly IEfRepository _repository;
        
        private readonly IProjectLogic _projectLogic;
        
        private readonly ICategoryLogic _categoryLogic;
        
        private readonly IUserLogic _userLogic;

        public ProjectManagementLogic(IEfRepository repository, IProjectLogic profileLogic, ICategoryLogic categoryLogic, IUserLogic userLogic)
        {
            _repository = repository;
            _projectLogic = profileLogic;
            _categoryLogic = categoryLogic;
            _userLogic = userLogic;
        }
        
        public async Task Add(User user, ProjectViewModel projectViewModel)
        {
            user = await _userLogic.Get(user.Id);
            
            var disposableResult = await _categoryLogic.SetRepository(_repository).GetOrCreate(projectViewModel.Categories);

            var project = await _projectLogic.SetRepository(_repository).Save(new Project
            {
                User = user,
                Title = projectViewModel.Title,
                Description = projectViewModel.Description,
                CreatedOn = DateTimeOffset.Now
            });

            project.ProjectCategoryRelationships = disposableResult
                .Select(x => new ProjectCategoryRelationship
                {
                    ProjectId = project.Id,
                    CategoryId = x.Id
                }).ToList();

            await _projectLogic.SetRepository(_repository).Update(project.Id, project);
        }

        public async Task Update(User user, ProjectViewModel projectViewModel)
        {
            user = await _userLogic.Get(user.Id);
            
            var disposableResult = await _categoryLogic.SetRepository(_repository).GetOrCreate(projectViewModel.Categories);
            
            var categories = disposableResult
                .Select(x => new ProjectCategoryRelationship
                {
                    ProjectId = projectViewModel.Id,
                    CategoryId = x.Id
                }).ToList();
            
            await _projectLogic.SetRepository(_repository).For(user).Update(projectViewModel.Id, project =>
            {
                project.Title = projectViewModel.Title;
                project.Description = projectViewModel.Description;
                project.ProjectCategoryRelationships = categories;
            });
        }
        
        public async Task Delete(User user, int projectId)
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
                Votes = project.Votes,
                CreatedOn = project.CreatedOn
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
                    Votes = project.Votes,
                    CreatedOn = project.CreatedOn
                };
            }).ToList();
        }
    }
}