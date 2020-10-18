using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Api.PasswordReset
{
    public class PasswordResetRequestViewModel
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}