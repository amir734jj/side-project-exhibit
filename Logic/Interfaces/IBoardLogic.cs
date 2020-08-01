using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.Enums;
using Models.ViewModels.Api;

namespace Logic.Interfaces
{
    public interface IBoardLogic
    {
        Task<BoardViewModels> Collect(int page, Sort sort, Order order, int pageSize, string category, string keyword);

        Task<Project> Vote(int ideaId, int userId, Vote vote);
    }
}