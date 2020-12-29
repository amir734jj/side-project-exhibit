using System;
using System.Linq;
using System.Threading.Tasks;
using Logic.Extensions;
using Logic.Interfaces;
using Models.Entities;
using Models.Enums;
using Models.Extensions;
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

            var allCategories = ideas.SelectMany(x => x.ProjectCategoryRelationships.Select(y => y.Category)).ToList();

            if (!string.IsNullOrWhiteSpace(category))
            {
                ideas = ideas.Where(x => x.ProjectCategoryRelationships.Any(y => y.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                ideas = ideas.Where(x =>
                    x.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    x.ProjectCategoryRelationships.Select(y => y.Category.Name)
                        .Contains(keyword, StringComparer.OrdinalIgnoreCase) ||
                    x.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            ideas = sort switch
            {
                Sort.Date => order switch
                {
                    Order.Ascending => ideas.OrderBy(x => x.CreatedOn).ToList(),
                    Order.Descending => ideas.OrderByDescending(x => x.CreatedOn).ToList(),
                    _ => ideas
                },
                Sort.Vote => order switch
                {
                    Order.Ascending => ideas.OrderBy(x => x.Votes.Sum(y => y.Value.IntValue())).ToList(),
                    Order.Descending => ideas.OrderByDescending(x => x.Votes.Sum(y => y.Value.IntValue())).ToList(),
                    _ => ideas
                },
                _ => ideas
            };

            var paginatedResult = ideas
                .Skip(pageSize * index)
                .Take(pageSize)
                .ToList();
            
            return new BoardViewModels
            {
                Projects = paginatedResult ,
                CurrentPage = 1,
                Categories = ideas.SelectMany(x => x.ProjectCategoryRelationships.Select(y => y.Category)).DistinctBy(x => x.Name).ToList(),
                Pages = (int) Math.Ceiling(1.0 * ideas.Count / pageSize),
                AllCategories = allCategories
            };
        }

        public async Task<Project> AddComment(int projectId, User user, CommentViewModel commentViewModel)
        {
            user = await _userLogic.Get(user.Id);
            
            await _projectLogic.For(user).Update(projectId, p =>
            {
                p.Comments.Add(new Comment
                {
                    CreatedOn = DateTimeOffset.Now,
                    Project = p,
                    Text = commentViewModel.Comment,
                    User = user
                });
            });
            
            var project = await _projectLogic.For(user).Get(projectId);

            await _userLogic.Update(project.User.Id, x =>
            {
                x.UserNotifications.Add(new UserNotification
                {
                    Subject = "New Comment",
                    Text = $"New comment by @{user.UserName} for project {project.Title}",
                    DateTime = DateTimeOffset.Now,
                    Collected = false
                });
            });

            return project;
        }

        public async Task<Project> DeleteComment(int projectId, User user, int commentId)
        {
            return await _projectLogic.For(user).Update(projectId, project =>
            {
                project.Comments = project.Comments.Where(x => x.Id != commentId).ToList();
            });
        }

        public async Task<Project> EditComment(int projectId, User user, int commentId, CommentViewModel comment)
        {
            return await _projectLogic.For(user).Update(projectId, project =>
            {
                project.Comments = project.Comments.Select(x =>
                {
                    if (x.Id == commentId)
                    {
                        x.Text = comment.Comment;
                    }

                    return x;
                }).ToList();
            });
        }

        public async Task<Project> Vote(int projectId, User user, Vote vote)
        {
            var project = await _projectLogic.Get(projectId);

            // Cannot vote for my own project
            if (project.User.Id == user.Id)
            {
                return project;
            }

            var isNewVote = false;
            
            await _userLogic.Update(user.Id, user =>
            {
                var previousVote = user.Votes.FirstOrDefault(y => y.Project.Id == projectId);

                if (previousVote == null)
                {
                    isNewVote = true;
                    
                    user.Votes.Add(new UserVote { ProjectId = projectId, UserId = user.Id, Value = vote });
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

            if (isNewVote)
            {
                await _userLogic.Update(project.User.Id, x =>
                {
                    x.UserNotifications.Add(new UserNotification
                    {
                        Subject = $"New {vote.ToString()} vote",
                        Text = $"New {vote.ToString().ToLower()} vote by @{user.UserName} for project {project.Title}",
                        DateTime = DateTimeOffset.Now,
                        Collected = false
                    });
                });
            }

            return await _projectLogic.Get(projectId);
        }
    }
}