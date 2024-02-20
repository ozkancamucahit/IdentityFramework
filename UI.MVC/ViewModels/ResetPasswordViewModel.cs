using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UI.MVC.ViewModels
{
    public sealed class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords dont match")]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string Code { get; set; }
    }
}
