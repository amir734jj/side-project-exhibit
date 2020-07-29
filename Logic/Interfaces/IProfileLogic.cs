using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels.Api;

namespace Logic.Interfaces
{
    public interface IProfileLogic
    {
        Task<ProfileViewModel> Update(User user, ProfileViewModel profileViewModel);
    }
}