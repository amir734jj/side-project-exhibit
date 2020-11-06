using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dal.Interfaces;
using Logic.Interfaces;
using Models.Entities;

namespace Logic.Logic
{
    public class UserSetup : IUserSetup
    {
        private readonly IUserLogic _userLogic;
        
        private readonly IEmailServiceApi _emailServiceApi;

        public UserSetup(IUserLogic userLogic, IEmailServiceApi emailServiceApi)
        {
            _userLogic = userLogic;
            _emailServiceApi = emailServiceApi;
        }

        public async Task Setup(int userId)
        {
            var users = (await _userLogic.GetAll()).ToList();

            var user = users.FirstOrDefault(x => x.Id == userId) ??
                       throw new Exception($"Failed to find the user with userId = {userId}");

            await _userLogic.Update(user.Id, x => x.UserNotifications = new List<UserNotification>
            {
                new UserNotification
                {
                    Subject = "Welcome to Anahita.dev",
                    Text = $"Welcome {user.Name} to Anahita.dev",
                    DateTime = new DateTimeOffset(),
                    Collected = false
                }
            });

            await _emailServiceApi.SendEmailAsync(user.Email, "Welcome to Anahita.dev", $@"
                <p> Welcome {user.Name} </p>
                <p> With Anahita.dev you can showcase your side projects, get feedback, and get motivated to work on other projects. </p>
                <br />
                <p> You are user number {users.Count} of the website. Thank you for joining. </p>
                <p> Your privacy is very important and your information will <strong>never</strong> be sold to a third party. </p>
            ");
        }
    }
}