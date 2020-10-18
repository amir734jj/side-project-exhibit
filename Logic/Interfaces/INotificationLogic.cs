using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface INotificationLogic
    {
        Task<List<UserNotification>> Collect(User user);

        Task MarkAsSeen(User user);
    }
}