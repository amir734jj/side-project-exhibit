using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Api.PasswordReset
{
    public class PasswordResetViewModel
    {
        public int Id { get; set; }
        
        public string Token { get; set; }
        
        public string Username { get; set; }

        public string Email { get; set; }
        
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password should contain lower and upper case alphanumeric characters + special character")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password should contain lower and upper case alphanumeric characters + special character")]
        public string ConfirmPassword { get; set; }
    }
}