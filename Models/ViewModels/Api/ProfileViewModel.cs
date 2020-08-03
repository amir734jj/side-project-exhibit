
using Models.Entities;
using Models.Enums;
using Newtonsoft.Json;

namespace Models.ViewModels.Api
{
    public class ProfileViewModel
    {
        public string Name { get; set; }

        public string Email { get; set; }
        
        public string Username { get; set; }

        public string Description { get; set; }
        
        public UserRoleEnum Role { get; set; }

        public ProfileViewModel()
        {
        }

        public ProfileViewModel(User user) : this()
        {
            if (user == null) return;

            Name = user.Name;
            Email = user.Email;
            Description = user.Description;
            Username = user.UserName;
            Role = user.UserRole;
        }
    }
}