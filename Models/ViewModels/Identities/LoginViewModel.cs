using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Identities
{
    /// <summary>
    ///     Login view model
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [MinLength(6)]
        public string Username { get; set; }
        
        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password { get; set; }
    }
}