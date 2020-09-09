using System.Threading.Tasks;
using Models.Entities;

namespace Logic.Interfaces
{
    public interface IPasswordResetLogic
    {
        Task SendPasswordResetEmail(User user, string passwordResetToken);
    }
}