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
        private const int PageSize = 10;
        
        private readonly IProjectLogic _projectLogic;
        
        private readonly IUserLogic _userLogic;

        public BoardLogic(IProjectLogic projectLogic, IUserLogic userLogic)
        {
            _projectLogic = projectLogic;
            _userLogic = userLogic;
        }
        
        public async Task<BoardViewModels> Collect(int pageNumber, Sort sort, Order order)
        {
            var ideas = (await _projectLogic.GetAll()).ToList();

            return new BoardViewModels
            {
                Projects = ideas
                    .Skip(PageSize * pageNumber - 1)
                    .Take(PageSize)
                    .ToList(),
                Pages = (int) Math.Ceiling(1.0 * ideas.Count / pageNumber)
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
                    previousVote.Value = vote;
                }
            });

            return await _projectLogic.Get(projectId);
        }
    }
}