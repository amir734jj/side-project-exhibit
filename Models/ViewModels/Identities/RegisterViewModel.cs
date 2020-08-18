using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Identities
{
    /// <summary>
    ///     Register view model
    /// </summary>
    public class RegisterViewModel
    {
        [Required] public string Name { get; set; }

        [Required] [MinLength(6)] public string Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            ErrorMessage = "Password should contain lower and upper case alphanumeric characters + special character")]
        public string ConfirmPassword { get; set; }

        [Required] [EmailAddress] public string Email { get; set; }
    }
}