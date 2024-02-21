using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UI.MVC.Models
{
    public sealed class AppUser : IdentityUser
    {
        [Required]
        [Length(4, 20)]
        public string NickName { get; set; } = String.Empty;

        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem> RoleList{ get; set; }
    }
}
