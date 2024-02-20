using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models
{
    public sealed class AppUser : IdentityUser
    {
        [Required]
        [Length(4, 20)]
        public string NickName { get; set; } = String.Empty;
    }
}
