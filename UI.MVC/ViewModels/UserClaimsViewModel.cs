namespace UI.MVC.ViewModels
{
    public sealed class UserClaimsViewModel
    {

        public string UserId { get; set; }

        public List<UserClaim> Claims { get; set; }

        public UserClaimsViewModel()
        {
            Claims = new();
        }


    }

    public sealed class UserClaim
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }


    }
}
