using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Identities
{
    /// <summary>
    ///     Register view model
    /// </summary>
    public class RegisterViewModel
    {
        public string Name { get; set; }

        public string Username { get; set; }
        
        public string Password { get; set; }
        
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
        
        public string Email { get; set; }
    }
}