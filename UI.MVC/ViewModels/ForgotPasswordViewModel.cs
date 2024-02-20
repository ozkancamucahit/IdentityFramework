using System.ComponentModel.DataAnnotations;

namespace UI.MVC.ViewModels
{
    public sealed class ForgotPasswordViewModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }



    }
}
