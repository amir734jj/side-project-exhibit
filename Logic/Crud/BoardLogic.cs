using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Entities;
using Models.Enums;
using Models.ViewModels.Api;

namespace Logic.Crud
{
    public class BoardLogic : IBoardLogic
    {
        private readonly IProjectLogic _projectLogic;
        
        private readonly IUserLogic _userLogic;

        public BoardLogic(IProjectLogic projectLogic, IUserLogic userLogic)
        {
            _projectLogic = projectLogic;
            _userLogic = userLogic;
        }
        
        public async Task<BoardViewModels> Collect(int index, Sort sort, Order order, int pageSize, string category, string keyword)
        {
            var ideas = (await _projectLogic.GetAll()).ToList();

            if (!string.IsNullOrWhiteSpace(category))
            {
                ideas = ideas.Where(x => x.ProjectCategoryRelationships.Any(y => y.Category.Name == category)).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ideas = ideas.Where(x => x.Title.Contains(keyword) || x.Description.Contains(keyword)).ToList();
            }

            return new BoardViewModels
            {
                Projects = ideas
                    .Skip(pageSize * index)
                    .Take(pageSize)
                    .ToList(),
                CurrentPage = 1,
                Categories = ideas.SelectMany(x => x.ProjectCategoryRelationships.Select(y => y.Category)).ToList(),
                Pages = (int) Math.Ceiling(1.0 * ideas.Count / pageSize)
            };
        }

        public async Task<Project> Vote(int projectId, int userId, Vote vote)
        {
            await _userLogic.Update(userId, user =>
            {
                var previousVote = user.Votes.FirstOrDefault(y => y.Project.Id == projectId);

                if (previousVote == null)
                {
                    user.Votes.Add(new UserVote { ProjectId = projectId, UserId = userId, Value = vote });
                }
                else
                {
                    if (previousVote.Value == vote)
                    {
                        user.Votes.Remove(previousVote);
                    }
                    else
                    {
                        previousVote.Value = vote;
                    }
                }
            });

            return await _projectLogic.Get(projectId);
        }
    }
}