using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Logic
{
    public class NotificationLogic : INotificationLogic
    {
        private readonly IUserLogic _userLogic;

        public NotificationLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        public async Task<List<UserNotification>> Collect(User user)
        {
            var notifications = (await _userLogic.Get(user.Id)).UserNotifications ?? new List<UserNotification>();

            return notifications;
        }

        public async Task MarkAsSeen(User user)
        {
            user.UserNotifications = user.UserNotifications.Select(x => new UserNotification
            {
                Subject = x.Subject,
                Text = x.Text,
                DateTime = x.DateTime,
                Collected = true
            }).ToList();

            await _userLogic.Update(user.Id, user);
        }
    }
}