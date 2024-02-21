using System.Security.Claims;

namespace UI.MVC.Models
{
    public static class ClaimStore
    {
        public static List<Claim> claimList = new()
        {
            new Claim("Create", "Create"),
            new Claim("Edit", "Edit"),
            new Claim("Delete", "Delete")
        };





    }
}
