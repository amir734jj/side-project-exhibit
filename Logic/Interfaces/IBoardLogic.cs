using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.Enums;

namespace Logic.Interfaces
{
    public interface IBoardLogic
    {
        Task<List<Project>> Collect(int page, Sort sort, Order order);

        Task<Project> Vote(int ideaId, int userId, Vote vote);
    }
}