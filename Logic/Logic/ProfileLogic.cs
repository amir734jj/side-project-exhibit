using System.Threading.Tasks;
using Logic.Interfaces;
using Models;
using Models.Entities;
using Models.ViewModels.Api;

namespace Logic.Logic
{
    public class ProfileLogic : IProfileLogic
    {
        private readonly IUserLogic _userLogic;

        public ProfileLogic(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }

        public async Task<ProfileViewModel> Update(User user, ProfileViewModel profileViewModel)
        {
            return new ProfileViewModel(await _userLogic.Update(user.Id, entity =>
            {
                entity.Name = profileViewModel.Name;
                entity.Description = profileViewModel.Description;
            }));
        }
    }
}