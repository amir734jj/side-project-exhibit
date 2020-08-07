using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace Models.ViewModels.Config
{
    public class GlobalConfigViewModel
    {
        [Display(Name = "Email Test Mode")]
        public bool EmailTestMode { get; set; }

        [Display(Name = "Website Theme")]
        public string Theme { get; set; } = "default";
    }
}