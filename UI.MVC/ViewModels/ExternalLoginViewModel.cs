using System.ComponentModel.DataAnnotations;

namespace UI.MVC.ViewModels
{
    public sealed class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
